using DevTodoList.Shared.Constants;

namespace DevTodoList.Shared.DTOs.Requests;

/// <summary>버스 출퇴근 설정 생성/수정 요청</summary>
public class CreateBusCommuteSettingRequest
{
    public string Name { get; set; } = string.Empty;
    public int CommuteType { get; set; }
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
    public long? BusStationId { get; set; }
}
