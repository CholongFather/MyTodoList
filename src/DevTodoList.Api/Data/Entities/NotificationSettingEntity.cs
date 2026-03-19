using System.ComponentModel.DataAnnotations.Schema;
using DevTodoList.Shared.Constants;

namespace DevTodoList.Api.Data.Entities;

/// <summary>알림 설정 (싱글톤)</summary>
[Table("NotificationSettings")]
public class NotificationSettingEntity : EntityBase
{
    public bool IsEnabled { get; set; } = true;
    public string NotificationTime { get; set; } = NotificationDefaults.NotificationTime;
    public int DashboardRefreshIntervalSeconds { get; set; } = NotificationDefaults.DashboardRefreshIntervalSeconds;
    public int DueSoonDays { get; set; } = NotificationDefaults.DueSoonDays;

    /// <summary>공공데이터포털 GBIS API 인증키 (버스 도착정보 조회)</summary>
    public string? GbisApiKey { get; set; }

    /// <summary>일정 알리미 Teams Workflow URL</summary>
    public string? ScheduleWebhookUrl { get; set; }

    /// <summary>버스 알리미 Teams Workflow URL</summary>
    public string? BusWebhookUrl { get; set; }
}
