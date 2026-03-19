using DevTodoList.Api.Services;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using DevTodoList.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DevTodoList.Api.Controllers;

[ApiController]
[Route("api/bus-commute")]
public class BusCommuteController(
    BusCommuteService svc,
    TeamsWebhookService teamsSvc,
    GbisBusArrivalService gbisSvc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await svc.GetAllAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var dto = await svc.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBusCommuteSettingRequest req, CancellationToken ct)
        => Ok(await svc.CreateAsync(req, ct));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] CreateBusCommuteSettingRequest req, CancellationToken ct)
    {
        var dto = await svc.UpdateAsync(id, req, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await svc.DeleteAsync(id, ct) ? NoContent() : NotFound();

    [HttpPut("{id:long}/toggle")]
    public async Task<IActionResult> Toggle(long id, CancellationToken ct)
        => await svc.ToggleAsync(id, ct) ? Ok() : NotFound();

    /// <summary>특정 설정의 실시간 도착정보 조회</summary>
    [HttpGet("{id:long}/arrival")]
    public async Task<IActionResult> GetArrival(long id, CancellationToken ct)
    {
        var setting = await svc.GetByIdAsync(id, ct);
        if (setting is null) return NotFound();
        if (string.IsNullOrEmpty(setting.GbisStationId) || string.IsNullOrEmpty(setting.GbisRouteId) || !setting.GbisStaOrder.HasValue)
            return BadRequest(new { message = "GBIS 정류소 정보가 설정되지 않았습니다." });

        var arrival = await gbisSvc.GetArrivalAsync(setting.GbisStationId, setting.GbisRouteId, setting.GbisStaOrder.Value, ct);
        return arrival is not null ? Ok(arrival.ToDto()) : Ok(new BusArrivalInfoDto { Message = "운행 정보 없음" });
    }

    /// <summary>정류소의 모든 노선 실시간 도착정보 조회</summary>
    [HttpGet("arrivals/{stationId}")]
    public async Task<IActionResult> GetAllArrivals(string stationId, CancellationToken ct)
    {
        var arrivals = await gbisSvc.GetAllArrivalsAsync(stationId, ct);
        return Ok(arrivals.Select(a => a.ToDto()).ToList());
    }

    /// <summary>오늘 알림 넘기기 (LastNotifiedAt을 오늘 자정으로 설정)</summary>
    [HttpPost("{id:long}/skip-today")]
    public async Task<IActionResult> SkipToday(long id, CancellationToken ct)
    {
        var result = await svc.SkipTodayAsync(id, ct);
        if (!result) return NotFound();
        return Ok(new { success = true, message = "오늘 알림을 건너뜁니다." });
    }

    /// <summary>오늘 알림 넘기기 (GET - 링크 클릭용)</summary>
    [HttpGet("{id:long}/skip-today")]
    public async Task<IActionResult> SkipTodayGet(long id, CancellationToken ct)
    {
        var result = await svc.SkipTodayAsync(id, ct);
        // 브라우저에서 접근 시 간단한 HTML 응답
        var message = result ? "오늘 알림을 건너뜁니다." : "설정을 찾을 수 없습니다.";
        return Content($"<html><body style='font-family:sans-serif;text-align:center;padding:40px'><h2>{message}</h2></body></html>", "text/html");
    }

    /// <summary>테스트 알림 발송 (실시간 도착정보 포함)</summary>
    [HttpPost("test-notification")]
    public async Task<IActionResult> TestNotification([FromBody] TestNotificationRequest req, CancellationToken ct)
    {
        var setting = await svc.GetByIdAsync(req.SettingId, ct);
        if (setting is null) return NotFound();

        var label = setting.CommuteType.ToKorean();

        // 실시간 도착정보 조회
        string? arrivalInfo = null;
        if (!string.IsNullOrEmpty(setting.GbisStationId) &&
            !string.IsNullOrEmpty(setting.GbisRouteId) &&
            setting.GbisStaOrder.HasValue)
        {
            try
            {
                var arrival = await gbisSvc.GetArrivalAsync(
                    setting.GbisStationId, setting.GbisRouteId, setting.GbisStaOrder.Value, ct);
                arrivalInfo = arrival is not null
                    ? BusNotificationBackgroundService.FormatArrivalSummary(arrival)
                    : null;
            }
            catch { /* 도착정보 실패해도 알림은 발송 */ }
        }

        var monitoringWindow = $"{setting.MonitoringStartTime} ~ {setting.MonitoringEndTime}";
        var result = await teamsSvc.SendBusNotificationAsync(
            setting.BusRouteNumber, setting.BusStopName,
            monitoringWindow, label, arrivalInfo, ct: ct);

        return result ? Ok(new { success = true }) : BadRequest(new { success = false, message = "발송 실패" });
    }
}
