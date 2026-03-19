using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>버스 출퇴근 알림 설정 CRUD 서비스</summary>
public class BusCommuteService(AppDbContext db)
{
    public async Task<List<BusCommuteSettingDto>> GetAllAsync(CancellationToken ct = default)
        => await db.BusCommuteSettings
            .Include(s => s.BusStation)
            .OrderBy(s => s.MonitoringStartTime)
            .Select(s => s.ToDto())
            .ToListAsync(ct);

    public async Task<BusCommuteSettingDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.BusCommuteSettings
            .Include(s => s.BusStation)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
        return entity?.ToDto();
    }

    public async Task<BusCommuteSettingDto> CreateAsync(CreateBusCommuteSettingRequest req, CancellationToken ct = default)
    {
        ValidateTimeFields(req);
        var entity = new BusCommuteSettingEntity
        {
            Name = req.Name,
            CommuteType = req.CommuteType,
            BusStopName = req.BusStopName,
            GbisStationId = req.GbisStationId,
            GbisRouteId = req.GbisRouteId,
            GbisStaOrder = req.GbisStaOrder,
            BusRouteNumber = req.BusRouteNumber,
            MonitoringStartTime = req.MonitoringStartTime,
            MonitoringEndTime = req.MonitoringEndTime,
            AlertMinutesBefore = req.AlertMinutesBefore,
            AlertThresholds = req.AlertThresholds,
            IsEnabled = req.IsEnabled,
            ActiveDays = req.ActiveDays,
            BusStationId = req.BusStationId
        };
        db.BusCommuteSettings.Add(entity);
        await db.SaveChangesAsync(ct);
        // Include로 재조회하여 BusStation 정보 포함
        await db.Entry(entity).Reference(e => e.BusStation).LoadAsync(ct);
        return entity.ToDto();
    }

    public async Task<BusCommuteSettingDto?> UpdateAsync(long id, CreateBusCommuteSettingRequest req, CancellationToken ct = default)
    {
        ValidateTimeFields(req);
        var entity = await db.BusCommuteSettings.FindAsync([id], ct);
        if (entity is null) return null;

        entity.Name = req.Name;
        entity.CommuteType = req.CommuteType;
        entity.BusStopName = req.BusStopName;
        entity.BusRouteNumber = req.BusRouteNumber;
        entity.GbisStationId = req.GbisStationId;
        entity.GbisRouteId = req.GbisRouteId;
        entity.GbisStaOrder = req.GbisStaOrder;
        entity.MonitoringStartTime = req.MonitoringStartTime;
        entity.MonitoringEndTime = req.MonitoringEndTime;
        entity.AlertMinutesBefore = req.AlertMinutesBefore;
        entity.AlertThresholds = req.AlertThresholds;
        entity.IsEnabled = req.IsEnabled;
        entity.ActiveDays = req.ActiveDays;
        entity.BusStationId = req.BusStationId;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await db.Entry(entity).Reference(e => e.BusStation).LoadAsync(ct);
        return entity.ToDto();
    }

    /// <summary>시간 형식 유효성 검증 (HH:mm)</summary>
    private static void ValidateTimeFields(CreateBusCommuteSettingRequest req)
    {
        if (!TimeSpan.TryParse(req.MonitoringStartTime, out _))
            throw new ArgumentException($"모니터링 시작 시간 형식이 올바르지 않습니다: '{req.MonitoringStartTime}' (HH:mm 형식 필요)");
        if (!TimeSpan.TryParse(req.MonitoringEndTime, out _))
            throw new ArgumentException($"모니터링 종료 시간 형식이 올바르지 않습니다: '{req.MonitoringEndTime}' (HH:mm 형식 필요)");
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.BusCommuteSettings.FindAsync([id], ct);
        if (entity is null) return false;
        db.BusCommuteSettings.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ToggleAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.BusCommuteSettings.FindAsync([id], ct);
        if (entity is null) return false;
        entity.IsEnabled = !entity.IsEnabled;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>오늘 알림 넘기기 (LastNotifiedAt을 오늘 자정으로 설정하여 금일 알림 중단)</summary>
    public async Task<bool> SkipTodayAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.BusCommuteSettings.FindAsync([id], ct);
        if (entity is null) return false;
        // 자정(00:00:00)으로 설정 → BackgroundService에서 스킵 판단
        entity.LastNotifiedAt = DateTime.Now.Date;
        await db.SaveChangesAsync(ct);
        return true;
    }
}
