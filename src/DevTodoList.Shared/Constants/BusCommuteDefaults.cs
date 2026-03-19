namespace DevTodoList.Shared.Constants;

/// <summary>버스 출퇴근 알림 기본값</summary>
public static class BusCommuteDefaults
{
    public const string MonitoringStartTime = "07:00";
    public const string MonitoringEndTime = "08:00";
    public const int AlertMinutesBefore = 10;
    public const int MaxAlertMinutesBefore = 60;
    /// <summary>다단계 알림 임계값 (분, 내림차순)</summary>
    public const string AlertThresholds = "15,10,5";
    public const bool IsEnabled = true;
    public const int ActiveDays = DayBitmasks.Weekdays;
}
