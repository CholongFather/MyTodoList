using DevTodoList.Shared.DTOs;

namespace DevTodoList.Api.Services;

/// <summary>BackupSettings 설정 섹션 읽기 (API 조회와 스케줄러가 같은 값을 보도록 한 곳에서 읽음)</summary>
public static class BackupSettingsExtensions
{
    public static BackupSettingsDto GetBackupSettings(this IConfiguration config)
    {
        var section = config.GetSection("BackupSettings");
        return new BackupSettingsDto
        {
            Enabled = section.GetValue("Enabled", true),
            ScheduleTime = section.GetValue("ScheduleTime", "02:00") ?? "02:00",
            RetentionDays = section.GetValue("RetentionDays", 30),
            MaxBackupCount = section.GetValue("MaxBackupCount", 10)
        };
    }
}
