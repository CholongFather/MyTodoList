using DevTodoList.Shared.DTOs;

namespace DevTodoList.Api.Services;

/// <summary>매일 지정 시간 자동 백업 BackgroundService</summary>
public class BackupSchedulerService(
    IServiceScopeFactory scopeFactory,
    IConfiguration config,
    ILogger<BackupSchedulerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("BackupSchedulerService 시작");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var settings = config.GetBackupSettings();
                if (settings.Enabled)
                {
                    var now = DateTime.Now;
                    var validTime = TimeSpan.TryParse(settings.ScheduleTime, out var scheduleTime)
                        && scheduleTime >= TimeSpan.Zero && scheduleTime < TimeSpan.FromDays(1);
                    if (!validTime)
                    {
                        // 시각을 읽지 못하면 대기 없이 무한 반복하므로 1시간 뒤 설정을 다시 확인
                        logger.LogWarning("BackupSettings:ScheduleTime 값을 읽을 수 없습니다: {ScheduleTime}", settings.ScheduleTime);
                        await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                        continue;
                    }

                    var nextRun = now.Date.Add(scheduleTime);
                    if (nextRun <= now)
                        nextRun = nextRun.AddDays(1);

                    var delay = nextRun - now;
                    logger.LogDebug("다음 자동 백업: {NextRun} ({Delay})", nextRun, delay);

                    await Task.Delay(delay, stoppingToken);

                    // 자동 백업 실행
                    await RunBackupAsync(settings, stoppingToken);
                }
                else
                {
                    // 비활성 상태면 1시간마다 설정 재확인
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "자동 백업 중 오류 발생");
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }

    private async Task RunBackupAsync(BackupSettingsDto settings, CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var backupService = scope.ServiceProvider.GetRequiredService<DatabaseBackupService>();

        await backupService.CreateBackupAsync("자동 백업", "Auto", ct);
        await backupService.CleanupOldBackupsAsync(settings.RetentionDays, settings.MaxBackupCount, ct);

        logger.LogInformation("자동 백업 완료");
    }
}
