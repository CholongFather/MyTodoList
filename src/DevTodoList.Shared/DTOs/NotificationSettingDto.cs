namespace DevTodoList.Shared.DTOs;

public class NotificationSettingDto
{
    public bool IsEnabled { get; set; } = true;
    public string NotificationTime { get; set; } = "09:00";
    public int DashboardRefreshIntervalSeconds { get; set; } = 300;
    public int DueSoonDays { get; set; } = 1;
    /// <summary>공공데이터포털 GBIS API 인증키</summary>
    public string? GbisApiKey { get; set; }

    /// <summary>일정 알리미 Teams Workflow URL</summary>
    public string? ScheduleWebhookUrl { get; set; }

    /// <summary>버스 알리미 Teams Workflow URL</summary>
    public string? BusWebhookUrl { get; set; }
}
