using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs;

/// <summary>버스 정류장 DTO</summary>
public class BusStationDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BusStopName { get; set; } = string.Empty;
    public string BusRouteNumber { get; set; } = string.Empty;
    public string? GbisStationId { get; set; }
    public string? GbisRouteId { get; set; }
    public int? GbisStaOrder { get; set; }
    public BusDirection Direction { get; set; }
    public int SortOrder { get; set; }
}
