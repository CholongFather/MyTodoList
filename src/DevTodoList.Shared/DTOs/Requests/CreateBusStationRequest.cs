namespace DevTodoList.Shared.DTOs.Requests;

/// <summary>버스 정류장 생성/수정 요청</summary>
public class CreateBusStationRequest
{
    public string Name { get; set; } = string.Empty;
    public string BusStopName { get; set; } = string.Empty;
    public string BusRouteNumber { get; set; } = string.Empty;
    public string? GbisStationId { get; set; }
    public string? GbisRouteId { get; set; }
    public int? GbisStaOrder { get; set; }
    public int Direction { get; set; }
}
