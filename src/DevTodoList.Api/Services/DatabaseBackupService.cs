using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.DTOs;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

public class DatabaseBackupService(AppDbContext db, IConfiguration config, ILogger<DatabaseBackupService> logger)
{
    /// <summary>DB 파일 절대경로</summary>
    private string GetDbFilePath()
    {
        var connStr = config.GetConnectionString("DefaultConnection") ?? "Data Source=devtodolist.db";
        var builder = new SqliteConnectionStringBuilder(connStr);
        var dataSource = builder.DataSource;
        return Path.IsPathRooted(dataSource) ? dataSource : Path.Combine(AppContext.BaseDirectory, dataSource);
    }

    /// <summary>백업 디렉토리 (DB 파일 옆 backups/)</summary>
    private string GetBackupDirectory()
    {
        var dir = Path.Combine(Path.GetDirectoryName(GetDbFilePath())!, "backups");
        Directory.CreateDirectory(dir);
        return dir;
    }

    public async Task<DatabaseBackupDto> CreateBackupAsync(string? description, string backupType = "Manual", CancellationToken ct = default)
    {
        var dbPath = GetDbFilePath();
        if (!File.Exists(dbPath))
            throw new FileNotFoundException("DB 파일을 찾을 수 없습니다.", dbPath);

        var fileName = $"devtodolist_{DateTime.Now:yyyyMMdd_HHmmss}.db";
        var backupPath = Path.Combine(GetBackupDirectory(), fileName);

        // SQLite 온라인 백업 (WAL 안전)
        await using var srcConn = new SqliteConnection($"Data Source={dbPath}");
        await srcConn.OpenAsync(ct);
        await using var destConn = new SqliteConnection($"Data Source={backupPath}");
        await destConn.OpenAsync(ct);
        srcConn.BackupDatabase(destConn);
        await destConn.CloseAsync();
        await srcConn.CloseAsync();

        var fileInfo = new FileInfo(backupPath);
        var entity = new DatabaseBackupEntity
        {
            FileName = fileName,
            FileSizeBytes = fileInfo.Length,
            BackupType = backupType,
            Description = description,
            BackupPath = backupPath,
            CreatedAt = DateTime.UtcNow
        };

        db.DatabaseBackups.Add(entity);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("DB 백업 생성: {FileName} ({Size} bytes)", fileName, fileInfo.Length);
        return entity.ToDto();
    }

    public async Task<List<DatabaseBackupDto>> GetBackupsAsync(CancellationToken ct = default)
        => await db.DatabaseBackups
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new DatabaseBackupDto
            {
                Id = b.Id,
                FileName = b.FileName,
                FileSizeBytes = b.FileSizeBytes,
                BackupType = b.BackupType,
                Description = b.Description,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync(ct);

    public async Task<bool> DeleteBackupAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.DatabaseBackups.FindAsync([id], ct);
        if (entity is null) return false;

        // 파일 삭제
        if (File.Exists(entity.BackupPath))
            File.Delete(entity.BackupPath);

        db.DatabaseBackups.Remove(entity);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("DB 백업 삭제: {FileName}", entity.FileName);
        return true;
    }

    public async Task<DatabaseBackupDto> RestoreBackupAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.DatabaseBackups.FindAsync([id], ct);
        if (entity is null)
            throw new KeyNotFoundException($"백업을 찾을 수 없습니다. (ID: {id})");

        if (!File.Exists(entity.BackupPath))
            throw new FileNotFoundException("백업 파일이 존재하지 않습니다.", entity.BackupPath);

        // 복원 전 현재 DB를 자동 백업
        await CreateBackupAsync("복원 전 자동 백업", "Auto", ct);

        // 복원하면 DatabaseBackups 표도 스냅샷 시점으로 돌아가므로 복원 전 목록을 보관
        var beforeRows = await db.DatabaseBackups.AsNoTracking().ToListAsync(ct);
        var restoredFileName = entity.FileName;

        var dbPath = GetDbFilePath();

        // 현재 DB 연결 해제 후 파일 교체
        // SQLite 백업 API로 복원 (안전)
        await using var srcConn = new SqliteConnection($"Data Source={entity.BackupPath}");
        await srcConn.OpenAsync(ct);
        await using var destConn = new SqliteConnection($"Data Source={dbPath}");
        await destConn.OpenAsync(ct);
        srcConn.BackupDatabase(destConn);
        await destConn.CloseAsync();
        await srcConn.CloseAsync();

        // 복원 전 추적 상태와 표 내용이 어긋나므로 초기화 후, 스냅샷에 없는 백업 행을 다시 등록
        // 파일 교체가 끝난 뒤에는 요청이 취소되어도 목록 복구를 끝까지 수행 (중단되면 백업 행이 유실됨)
        var recoverCt = CancellationToken.None;
        db.ChangeTracker.Clear();
        var restoredNames = await db.DatabaseBackups.Select(b => b.FileName).ToListAsync(recoverCt);
        var missingRows = beforeRows.Where(b => !restoredNames.Contains(b.FileName)).ToList();
        foreach (var row in missingRows)
        {
            row.Id = 0;
            db.DatabaseBackups.Add(row);
        }

        if (missingRows.Count > 0)
            await db.SaveChangesAsync(recoverCt);

        var restored = await db.DatabaseBackups.AsNoTracking().FirstAsync(b => b.FileName == restoredFileName, recoverCt);

        logger.LogInformation("DB 복원 완료: {FileName} (목록 복구 {Count}건)", restoredFileName, missingRows.Count);
        return restored.ToDto();
    }

    /// <summary>오래된 백업 정리</summary>
    public async Task CleanupOldBackupsAsync(int retentionDays, int maxCount, CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow.AddDays(-retentionDays);
        var oldBackups = await db.DatabaseBackups
            .Where(b => b.CreatedAt < cutoff)
            .ToListAsync(ct);

        foreach (var backup in oldBackups)
        {
            if (File.Exists(backup.BackupPath))
                File.Delete(backup.BackupPath);
            db.DatabaseBackups.Remove(backup);
        }

        // 보관일 초과분을 먼저 확정해야 아래 개수 계산과 조회에 반영됨
        await db.SaveChangesAsync(ct);

        // maxCount 초과분 삭제
        var remainCount = await db.DatabaseBackups.CountAsync(ct);
        // maxCount가 0 이하로 잘못 설정돼도 방금 만든 백업까지 지우지 않도록 최소 1개 유지
        var excessCount = remainCount - Math.Max(maxCount, 1);
        var excess = excessCount > 0
            ? await db.DatabaseBackups.OrderBy(b => b.CreatedAt).ThenBy(b => b.Id).Take(excessCount).ToListAsync(ct)
            : [];

        foreach (var backup in excess)
        {
            if (File.Exists(backup.BackupPath))
                File.Delete(backup.BackupPath);
            db.DatabaseBackups.Remove(backup);
        }

        if (excess.Count > 0)
            await db.SaveChangesAsync(ct);

        var removedCount = oldBackups.Count + excess.Count;
        if (removedCount > 0)
            logger.LogInformation("오래된 백업 {Count}건 정리 완료", removedCount);
    }
}
