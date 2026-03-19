namespace DevTodoList.Shared.DTOs;

/// <summary>버스 실시간 도착정보 DTO</summary>
public class BusArrivalInfoDto
{
    public string RouteName { get; set; } = "";
    public string RouteId { get; set; } = "";
    public int StaOrder { get; set; }
    /// <summary>첫번째 차량 도착 예상 시간(분)</summary>
    public int PredictTime1 { get; set; }
    /// <summary>두번째 차량 도착 예상 시간(분)</summary>
    public int PredictTime2 { get; set; }
    /// <summary>첫번째 차량 현재 위치 (N정거장 전)</summary>
    public int LocationNo1 { get; set; }
    public int LocationNo2 { get; set; }
    /// <summary>빈자리 수</summary>
    public int RemainSeat1 { get; set; }
    public int RemainSeat2 { get; set; }
    public string? PlateNo1 { get; set; }
    public string? PlateNo2 { get; set; }
    public string? StationName1 { get; set; }
    public string? StationName2 { get; set; }
    /// <summary>요약 텍스트</summary>
    public string? Summary { get; set; }
    /// <summary>운행 정보 없음 메시지</summary>
    public string? Message { get; set; }
}
