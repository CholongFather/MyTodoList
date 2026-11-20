namespace DevTodoList.Shared.DTOs;

public class BackupSettingsDto
{
    public bool Enabled { get; set; }
    public string ScheduleTime { get; set; } = "02:00";
    public int RetentionDays { get; set; } = 30;
    public int MaxBackupCount { get; set; } = 10;
}
