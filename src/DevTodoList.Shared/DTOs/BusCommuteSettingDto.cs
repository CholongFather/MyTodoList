using DevTodoList.Shared.Constants;
using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs;

/// <summary>버스 출퇴근 알림 설정 DTO</summary>
public class BusCommuteSettingDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public CommuteType CommuteType { get; set; }
    public string BusStopName { get; set; } = string.Empty;
    public string BusRouteNumber { get; set; } = string.Empty;
    public string? GbisStationId { get; set; }
    public string? GbisRouteId { get; set; }
    public int? GbisStaOrder { get; set; }
    public string MonitoringStartTime { get; set; } = BusCommuteDefaults.MonitoringStartTime;
    public string MonitoringEndTime { get; set; } = BusCommuteDefaults.MonitoringEndTime;
    public int AlertMinutesBefore { get; set; } = BusCommuteDefaults.AlertMinutesBefore;
    public string AlertThresholds { get; set; } = BusCommuteDefaults.AlertThresholds;
    public bool IsEnabled { get; set; } = BusCommuteDefaults.IsEnabled;
    public int ActiveDays { get; set; } = BusCommuteDefaults.ActiveDays;
    public DateTime? LastNotifiedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? BusStationId { get; set; }
    public string? BusStationName { get; set; }
}
