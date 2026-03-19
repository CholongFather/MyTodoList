using DevTodoList.Api.Services;
using DevTodoList.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DevTodoList.Api.Controllers;

[ApiController]
[Route("api/settings")]
public class SettingsController(NotificationSettingService svc, TeamsWebhookService teamsSvc) : ControllerBase
{
    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications(CancellationToken ct)
        => Ok(await svc.GetAsync(ct));

    [HttpPut("notifications")]
    public async Task<IActionResult> UpdateNotifications([FromBody] NotificationSettingDto dto, CancellationToken ct)
        => Ok(await svc.UpdateAsync(dto, ct));

    /// <summary>Teams 웹훅 테스트 알림 발송</summary>
    [HttpPost("test-teams")]
    public async Task<IActionResult> TestTeamsNotification([FromBody] TestTeamsRequest? req, CancellationToken ct)
    {
        var title = req?.Title ?? "DevTodoList 테스트 알림";
        var message = req?.Message ?? $"Teams 연동 테스트입니다. ({DateTime.Now:yyyy-MM-dd HH:mm:ss})";
        var channel = req?.Channel ?? "schedule";
        var result = await teamsSvc.SendScheduleNotificationAsync(title, message, channel, ct);
        return result ? Ok(new { success = true, channel }) : BadRequest(new { success = false, message = "발송 실패 - 웹훅 URL을 확인하세요." });
    }
}

public class TestTeamsRequest
{
    public string? Title { get; set; }
    public string? Message { get; set; }
    /// <summary>채널 키 (Power Automate Switch 분기용)</summary>
    public string? Channel { get; set; }
}
