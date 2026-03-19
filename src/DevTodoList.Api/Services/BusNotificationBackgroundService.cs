using System.Collections.Concurrent;
using DevTodoList.Api.Data;
using DevTodoList.Shared.Constants;
using DevTodoList.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>
/// 버스 알림 백그라운드 스케줄러 (30초 주기)
/// 모니터링 윈도우 내 GBIS 실시간 도착정보 기반 다단계 알림
/// - AlertThresholds (예: "15,10,5") 각 임계값마다 알림 발송
/// - 차량번호(plateNo) 기반으로 새 버스 감지 → 임계값 추적 리셋
/// - 오늘 스킵 플래그(LastNotifiedAt 자정 설정)로 알림 중단 가능
/// </summary>
public class BusNotificationBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<BusNotificationBackgroundService> logger) : BackgroundService
{
    /// <summary>설정별 알림 추적 정보 (차량번호 + 발송 완료된 임계값)</summary>
    private static readonly ConcurrentDictionary<long, NotifiedBusInfo> _notifiedBuses = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("BusNotificationBackgroundService 시작");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndNotifyAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "버스 알림 체크 중 오류 발생");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task CheckAndNotifyAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var teamsService = scope.ServiceProvider.GetRequiredService<TeamsWebhookService>();
        var gbisService = scope.ServiceProvider.GetRequiredService<GbisBusArrivalService>();
        var holidayService = scope.ServiceProvider.GetRequiredService<HolidayService>();

        var now = DateTime.Now;
        var today = now.Date;

        // 공휴일/연차이면 알림 스킵
        if (await holidayService.IsHolidayOrLeaveAsync(today, ct)) return;

        var currentDayBit = DayBitmasks.GetDayBit(now.DayOfWeek);
        var currentTime = now.TimeOfDay;

        var settings = await db.BusCommuteSettings
            .Include(s => s.BusStation)
            .Where(s => s.IsEnabled)
            .ToListAsync(ct);

        foreach (var setting in settings)
        {
            // 요일 체크
            if ((setting.ActiveDays & currentDayBit) == 0) continue;

            // "오늘 넘기기" 체크: LastNotifiedAt이 오늘 자정(00:00:00)이면 스킵
            if (setting.LastNotifiedAt.HasValue &&
                setting.LastNotifiedAt.Value.Date == today &&
                setting.LastNotifiedAt.Value.TimeOfDay == TimeSpan.Zero)
                continue;

            // 모니터링 시간 파싱
            if (!TimeSpan.TryParse(setting.MonitoringStartTime, out var windowStart)) continue;
            if (!TimeSpan.TryParse(setting.MonitoringEndTime, out var windowEnd)) continue;

            if (currentTime < windowStart || currentTime > windowEnd)
            {
                // 윈도우 밖이면 추적 정보 초기화
                _notifiedBuses.TryRemove(setting.Id, out _);
                continue;
            }

            // 정류장 마스터 폴백: BusStation FK가 있으면 마스터 데이터 사용
            var stationId = setting.BusStation?.GbisStationId ?? setting.GbisStationId;
            var routeId = setting.BusStation?.GbisRouteId ?? setting.GbisRouteId;
            var staOrder = setting.BusStation?.GbisStaOrder ?? setting.GbisStaOrder;
            var busStopName = setting.BusStation?.BusStopName ?? setting.BusStopName;
            var busRouteNumber = setting.BusStation?.BusRouteNumber ?? setting.BusRouteNumber;

            // GBIS 정보 없으면 스킵
            if (string.IsNullOrEmpty(stationId) ||
                string.IsNullOrEmpty(routeId) ||
                !staOrder.HasValue)
                continue;

            // GBIS 실시간 도착정보 조회
            BusArrivalInfo? arrival;
            try
            {
                arrival = await gbisService.GetArrivalAsync(
                    stationId, routeId, staOrder.Value, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "GBIS 도착정보 조회 실패: {Name}", setting.Name);
                continue;
            }

            if (arrival is null || arrival.PredictTime1 <= 0) continue;

            // 임계값 목록 파싱 (내림차순 정렬)
            var thresholds = ParseThresholds(setting.AlertThresholds, setting.AlertMinutesBefore);
            var eta = arrival.PredictTime1;

            // 현재 ETA 이상인 임계값 중 가장 작은 것 찾기
            // 예: thresholds=[15,10,5], ETA=7 → matchedThreshold=10
            var matchedThreshold = thresholds.Where(t => eta <= t).LastOrDefault();
            if (matchedThreshold <= 0) continue;

            // 차량 기반 추적 정보 확인
            var currentPlate = arrival.PlateNo1 ?? "";
            _notifiedBuses.TryGetValue(setting.Id, out var lastInfo);

            // 새 버스(다른 차량번호) 감지 시 추적 리셋
            if (lastInfo is not null && lastInfo.Date == today &&
                !string.IsNullOrEmpty(currentPlate) &&
                lastInfo.PlateNo != currentPlate)
            {
                lastInfo = null;
            }

            // 오늘 같은 차량의 해당 임계값에 이미 알림 발송했으면 스킵
            if (lastInfo is not null && lastInfo.Date == today &&
                lastInfo.PlateNo == currentPlate &&
                lastInfo.NotifiedThresholds.Contains(matchedThreshold))
                continue;

            var commuteLabel = ((CommuteType)setting.CommuteType).ToKorean();
            var arrivalSummary = FormatArrivalSummary(arrival);

            var monitoringWindow = $"{setting.MonitoringStartTime} ~ {setting.MonitoringEndTime}";
            var sent = await teamsService.SendBusNotificationAsync(
                busRouteNumber,
                busStopName,
                monitoringWindow,
                commuteLabel,
                arrivalSummary,
                ct: ct);

            if (sent)
            {
                // 임계값 발송 기록 업데이트
                if (lastInfo is null || lastInfo.Date != today || lastInfo.PlateNo != currentPlate)
                {
                    _notifiedBuses[setting.Id] = new NotifiedBusInfo(today, currentPlate, [matchedThreshold]);
                }
                else
                {
                    lastInfo.NotifiedThresholds.Add(matchedThreshold);
                }

                setting.LastNotifiedAt = now;
                await db.SaveChangesAsync(ct);
                logger.LogInformation(
                    "알림 발송: {Name} ({CommuteType}) - {BusRoute} [{Plate}] ETA {ETA}분 (임계값: {Threshold}분)",
                    setting.Name, commuteLabel, busRouteNumber,
                    currentPlate, eta, matchedThreshold);
            }
        }
    }

    /// <summary>AlertThresholds 문자열 파싱 → 내림차순 정렬된 int 배열</summary>
    internal static int[] ParseThresholds(string? thresholds, int fallbackMinutes)
    {
        if (!string.IsNullOrWhiteSpace(thresholds))
        {
            var parsed = thresholds.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => int.TryParse(s, out var v) ? v : 0)
                .Where(v => v > 0)
                .OrderByDescending(v => v)
                .ToArray();
            if (parsed.Length > 0) return parsed;
        }
        // 폴백: 기존 AlertMinutesBefore 단일 값
        return fallbackMinutes > 0 ? [fallbackMinutes] : [10];
    }

    /// <summary>1번째 차 + 2번째 차 정보 포함한 요약 생성</summary>
    public static string FormatArrivalSummary(BusArrivalInfo arrival)
    {
        var parts = new List<string>();

        // 1번째 차
        if (arrival.PredictTime1 > 0)
        {
            var seat = arrival.RemainSeat1 > 0 ? $", 빈자리 {arrival.RemainSeat1}석" : "";
            var plate = !string.IsNullOrEmpty(arrival.PlateNo1) ? $" [{arrival.PlateNo1}]" : "";
            parts.Add($"▶ {arrival.PredictTime1}분 후 ({arrival.LocationNo1}정거장 전{seat}){plate}");
        }

        // 2번째 차
        if (arrival.PredictTime2 > 0)
        {
            var seat = arrival.RemainSeat2 > 0 ? $", 빈자리 {arrival.RemainSeat2}석" : "";
            var plate = !string.IsNullOrEmpty(arrival.PlateNo2) ? $" [{arrival.PlateNo2}]" : "";
            parts.Add($"▷ 다음 {arrival.PredictTime2}분 후 ({arrival.LocationNo2}정거장 전{seat}){plate}");
        }

        return parts.Count > 0 ? string.Join("\n", parts) : "운행 정보 없음";
    }

    private record NotifiedBusInfo(DateTime Date, string PlateNo, HashSet<int> NotifiedThresholds);
}
