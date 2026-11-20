namespace DevTodoList.Shared.DTOs;

public class DatabaseBackupDto
{
    public long Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string BackupType { get; set; } = "Manual";
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>파일 크기 포맷 (예: "270 KB")</summary>
    public string FileSizeDisplay => FileSizeBytes switch
    {
        < 1024 => $"{FileSizeBytes} B",
        < 1024 * 1024 => $"{FileSizeBytes / 1024.0:F1} KB",
        _ => $"{FileSizeBytes / (1024.0 * 1024.0):F1} MB"
    };
}
