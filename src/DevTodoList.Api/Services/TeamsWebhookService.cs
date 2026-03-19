using System.Text;
using System.Text.Json;
using DevTodoList.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>Teams Workflow (Power Automate) 웹훅 알림 서비스</summary>
public class TeamsWebhookService(IHttpClientFactory httpFactory, IServiceScopeFactory scopeFactory, IConfiguration config, ILogger<TeamsWebhookService> logger)
{
    /// <summary>DB에서 채널별 URL 조회, 없으면 appsettings 폴백</summary>
    private async Task<string> ResolveUrlAsync(string channel)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var settings = await db.NotificationSettings.FirstOrDefaultAsync();

        var dbUrl = channel switch
        {
            "bus-commute" => settings?.BusWebhookUrl,
            _ => settings?.ScheduleWebhookUrl
        };

        if (!string.IsNullOrEmpty(dbUrl)) return dbUrl;

        // appsettings 폴백 (레거시)
        var commonUrl = config["TeamsWebhook:WebhookUrl"] ?? string.Empty;
        if (!string.IsNullOrEmpty(commonUrl)) return commonUrl;

        return channel switch
        {
            "bus-commute" => config["TeamsWebhook:BusCommuteUrl"] ?? string.Empty,
            _ => config["TeamsWebhook:ScheduleUrl"] ?? string.Empty
        };
    }

    /// <summary>API 기본 URL (skip-today 링크 생성용)</summary>
    private string ApiBaseUrl => config["ApiBaseUrl"] ?? "http://localhost:5100";

    /// <summary>버스 출퇴근 알림 Adaptive Card 발송</summary>
    public async Task<bool> SendBusNotificationAsync(
        string busRoute, string busStopName,
        string monitoringWindow, string commuteTypeLabel,
        string? arrivalInfo = null, bool includeSkipAction = false,
        CancellationToken ct = default)
    {
        var facts = new List<object>
        {
            new { title = "노선", value = busRoute },
            new { title = "정류장", value = busStopName },
            new { title = "모니터링", value = monitoringWindow },
            new { title = "현재 시간", value = DateTime.Now.ToString("HH:mm:ss") },
            new { title = "날짜", value = DateTime.Now.ToString("yyyy-MM-dd (ddd)") }
        };

        if (!string.IsNullOrEmpty(arrivalInfo))
            facts.Add(new { title = "실시간 도착", value = arrivalInfo });

        var url = await ResolveUrlAsync("bus-commute");
        return await SendAdaptiveCardAsync(
            url,
            "bus-commute",
            $"\ud83d\ude8c 버스 {commuteTypeLabel} 알림",
            facts.ToArray(),
            "곧 출발 시간입니다. 준비하세요!",
            ct);
    }

    /// <summary>일정 알림 Adaptive Card 발송 (할일 마감 등)</summary>
    public async Task<bool> SendScheduleNotificationAsync(
        string title, string message, string channel = "schedule", CancellationToken ct = default)
    {
        var url = await ResolveUrlAsync(channel);
        return await SendAdaptiveCardAsync(url, channel, title, null, message, ct);
    }

    /// <summary>Adaptive Card 공통 발송 (channel 키 포함)</summary>
    private async Task<bool> SendAdaptiveCardAsync(
        string webhookUrl, string channel, string title, object[]? facts, string? footerText, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(webhookUrl))
        {
            logger.LogWarning("Teams Webhook URL이 설정되지 않았습니다.");
            return false;
        }

        try
        {
            var cardBody = new List<object>
            {
                new { type = "TextBlock", size = "Medium", weight = "Bolder", text = title }
            };

            if (facts is { Length: > 0 })
            {
                cardBody.Add(new { type = "FactSet", facts });
            }

            if (!string.IsNullOrEmpty(footerText))
            {
                cardBody.Add(new { type = "TextBlock", text = footerText, wrap = true, color = "Attention" });
            }

            var payload = new Dictionary<string, object>
            {
                ["type"] = "message",
                ["channel"] = channel,
                ["attachments"] = new[]
                {
                    new
                    {
                        contentType = "application/vnd.microsoft.card.adaptive",
                        content = new
                        {
                            type = "AdaptiveCard",
                            version = "1.4",
                            body = cardBody.ToArray()
                        }
                    }
                }
            };

            using var client = httpFactory.CreateClient();
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(webhookUrl, content, ct);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Teams 웹훅 발송 실패: {StatusCode}, Channel: {Channel}", response.StatusCode, channel);
                return false;
            }

            logger.LogInformation("Teams 알림 발송 완료: {Title} → {Channel}", title, channel);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Teams 웹훅 발송 중 오류 (Channel: {Channel})", channel);
            return false;
        }
    }
}
