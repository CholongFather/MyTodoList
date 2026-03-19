using DevTodoList.Api.Data;
using DevTodoList.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>알림 설정 서비스</summary>
public class NotificationSettingService(AppDbContext db)
{
    public async Task<NotificationSettingDto> GetAsync(CancellationToken ct = default)
    {
        var entity = await db.NotificationSettings.FirstOrDefaultAsync(ct);
        return entity?.ToDto() ?? new NotificationSettingDto();
    }

    public async Task<NotificationSettingDto> UpdateAsync(NotificationSettingDto dto, CancellationToken ct = default)
    {
        var entity = await db.NotificationSettings.FirstOrDefaultAsync(ct);
        if (entity is null) return new NotificationSettingDto();

        entity.IsEnabled = dto.IsEnabled;
        entity.NotificationTime = dto.NotificationTime;
        entity.DashboardRefreshIntervalSeconds = dto.DashboardRefreshIntervalSeconds;
        entity.DueSoonDays = dto.DueSoonDays;
        entity.GbisApiKey = dto.GbisApiKey;
        entity.ScheduleWebhookUrl = dto.ScheduleWebhookUrl;
        entity.BusWebhookUrl = dto.BusWebhookUrl;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }
}
