using System.Net.Http.Json;
using System.Text.Json.Serialization;
using DevTodoList.Api.Data;
using DevTodoList.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>
/// GBIS 경기도 버스 도착정보 조회 서비스
/// stationId/routeId/staOrder는 경기버스정보(m.gbis.go.kr)에서 확인
/// API: https://apis.data.go.kr/6410000/busarrivalservice/v2
/// </summary>
public class GbisBusArrivalService(IHttpClientFactory httpFactory, AppDbContext db, IConfiguration config)
{
    private const string BaseUrl = "https://apis.data.go.kr/6410000/busarrivalservice/v2";

    /// <summary>DB 설정 우선, 없으면 appsettings 폴백</summary>
    private async Task<string> GetApiKeyAsync(CancellationToken ct)
    {
        var setting = await db.NotificationSettings.FirstOrDefaultAsync(ct);
        var key = setting?.GbisApiKey;
        if (!string.IsNullOrWhiteSpace(key)) return key;
        return config["GbisApiKey"] ?? string.Empty;
    }

    /// <summary>특정 정류소+노선의 도착정보 조회</summary>
    public async Task<BusArrivalInfo?> GetArrivalAsync(
        string stationId, string routeId, int staOrder, CancellationToken ct = default)
    {
        var apiKey = await GetApiKeyAsync(ct);
        if (string.IsNullOrWhiteSpace(apiKey)) return null;

        using var client = httpFactory.CreateClient();
        var url = $"{BaseUrl}/getBusArrivalItemv2?serviceKey={apiKey}&stationId={stationId}&routeId={routeId}&staOrder={staOrder}&format=json";

        var response = await client.GetFromJsonAsync<GbisArrivalItemResponse>(url, ct);
        var item = response?.Response?.MsgBody?.BusArrivalItem;
        if (item is null) return null;

        return new BusArrivalInfo
        {
            RouteName = item.RouteName?.ToString() ?? "",
            PredictTime1 = ParseInt(item.PredictTime1),
            PredictTime2 = ParseInt(item.PredictTime2),
            LocationNo1 = ParseInt(item.LocationNo1),
            LocationNo2 = ParseInt(item.LocationNo2),
            RemainSeat1 = ParseInt(item.RemainSeatCnt1),
            RemainSeat2 = ParseInt(item.RemainSeatCnt2),
            PlateNo1 = item.PlateNo1?.ToString(),
            PlateNo2 = item.PlateNo2?.ToString(),
            StationName1 = item.StationNm1?.ToString(),
            StationName2 = item.StationNm2?.ToString()
        };
    }

    /// <summary>정류소의 모든 노선 도착정보 조회</summary>
    public async Task<List<BusArrivalInfo>> GetAllArrivalsAsync(
        string stationId, CancellationToken ct = default)
    {
        var apiKey = await GetApiKeyAsync(ct);
        if (string.IsNullOrWhiteSpace(apiKey)) return [];

        using var client = httpFactory.CreateClient();
        var url = $"{BaseUrl}/getBusArrivalListv2?serviceKey={apiKey}&stationId={stationId}&format=json";

        var response = await client.GetFromJsonAsync<GbisArrivalListResponse>(url, ct);
        var items = response?.Response?.MsgBody?.BusArrivalList;
        if (items is null) return [];

        return items.Select(item => new BusArrivalInfo
        {
            RouteName = item.RouteName?.ToString() ?? "",
            RouteId = item.RouteId?.ToString() ?? "",
            StaOrder = ParseInt(item.StaOrder),
            PredictTime1 = ParseInt(item.PredictTime1),
            PredictTime2 = ParseInt(item.PredictTime2),
            LocationNo1 = ParseInt(item.LocationNo1),
            LocationNo2 = ParseInt(item.LocationNo2),
            RemainSeat1 = ParseInt(item.RemainSeatCnt1),
            RemainSeat2 = ParseInt(item.RemainSeatCnt2),
            PlateNo1 = item.PlateNo1?.ToString(),
            PlateNo2 = item.PlateNo2?.ToString(),
            StationName1 = item.StationNm1?.ToString(),
            StationName2 = item.StationNm2?.ToString()
        }).Where(a => a.PredictTime1 > 0).ToList();
    }

    private static int ParseInt(object? value)
    {
        if (value is null) return 0;
        return int.TryParse(value.ToString(), out var v) ? v : 0;
    }
}

/// <summary>도착 정보 결과</summary>
public class BusArrivalInfo
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

    /// <summary>알림 메시지용 요약</summary>
    public string ToSummary()
    {
        var parts = new List<string>();
        if (PredictTime1 > 0)
        {
            var seat = RemainSeat1 > 0 ? $", 빈자리 {RemainSeat1}석" : "";
            parts.Add($"{PredictTime1}분 후 ({LocationNo1}정거장 전{seat})");
        }
        if (PredictTime2 > 0)
            parts.Add($"다음 {PredictTime2}분 후");
        return parts.Count > 0 ? string.Join(" / ", parts) : "운행 정보 없음";
    }

    /// <summary>DTO 변환</summary>
    public BusArrivalInfoDto ToDto() => new()
    {
        RouteName = RouteName,
        RouteId = RouteId,
        StaOrder = StaOrder,
        PredictTime1 = PredictTime1,
        PredictTime2 = PredictTime2,
        LocationNo1 = LocationNo1,
        LocationNo2 = LocationNo2,
        RemainSeat1 = RemainSeat1,
        RemainSeat2 = RemainSeat2,
        PlateNo1 = PlateNo1,
        PlateNo2 = PlateNo2,
        StationName1 = StationName1,
        StationName2 = StationName2,
        Summary = ToSummary()
    };
}

#region GBIS API 응답 모델

internal class GbisArrivalItemResponse
{
    [JsonPropertyName("response")] public GbisResponse<GbisArrivalItemBody>? Response { get; set; }
}

internal class GbisArrivalListResponse
{
    [JsonPropertyName("response")] public GbisResponse<GbisArrivalListBody>? Response { get; set; }
}

internal class GbisResponse<T>
{
    [JsonPropertyName("msgHeader")] public GbisMsgHeader? MsgHeader { get; set; }
    [JsonPropertyName("msgBody")] public T? MsgBody { get; set; }
}

internal class GbisMsgHeader
{
    [JsonPropertyName("resultCode")] public int ResultCode { get; set; }
    [JsonPropertyName("resultMessage")] public string? ResultMessage { get; set; }
}

internal class GbisArrivalItemBody
{
    [JsonPropertyName("busArrivalItem")] public GbisArrivalItem? BusArrivalItem { get; set; }
}

internal class GbisArrivalListBody
{
    [JsonPropertyName("busArrivalList")] public List<GbisArrivalItem>? BusArrivalList { get; set; }
}

internal class GbisArrivalItem
{
    [JsonPropertyName("routeName")] public object? RouteName { get; set; }
    [JsonPropertyName("routeId")] public object? RouteId { get; set; }
    [JsonPropertyName("staOrder")] public object? StaOrder { get; set; }
    [JsonPropertyName("predictTime1")] public object? PredictTime1 { get; set; }
    [JsonPropertyName("predictTime2")] public object? PredictTime2 { get; set; }
    [JsonPropertyName("locationNo1")] public object? LocationNo1 { get; set; }
    [JsonPropertyName("locationNo2")] public object? LocationNo2 { get; set; }
    [JsonPropertyName("remainSeatCnt1")] public object? RemainSeatCnt1 { get; set; }
    [JsonPropertyName("remainSeatCnt2")] public object? RemainSeatCnt2 { get; set; }
    [JsonPropertyName("plateNo1")] public object? PlateNo1 { get; set; }
    [JsonPropertyName("plateNo2")] public object? PlateNo2 { get; set; }
    [JsonPropertyName("stationNm1")] public object? StationNm1 { get; set; }
    [JsonPropertyName("stationNm2")] public object? StationNm2 { get; set; }
}

#endregion
