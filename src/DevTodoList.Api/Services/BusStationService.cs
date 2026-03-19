using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>버스 정류장 마스터 서비스</summary>
public class BusStationService(AppDbContext db)
{
    public async Task<List<BusStationDto>> GetAllAsync(CancellationToken ct = default)
        => await db.BusStations
            .OrderBy(s => s.SortOrder)
            .Select(s => s.ToDto())
            .ToListAsync(ct);

    public async Task<BusStationDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.BusStations.FindAsync([id], ct);
        return entity?.ToDto();
    }

    public async Task<BusStationDto> CreateAsync(CreateBusStationRequest req, CancellationToken ct = default)
    {
        var maxSort = await db.BusStations.MaxAsync(s => (int?)s.SortOrder, ct) ?? 0;
        var entity = new BusStationEntity
        {
            Name = req.Name,
            BusStopName = req.BusStopName,
            BusRouteNumber = req.BusRouteNumber,
            GbisStationId = req.GbisStationId,
            GbisRouteId = req.GbisRouteId,
            GbisStaOrder = req.GbisStaOrder,
            Direction = req.Direction,
            SortOrder = maxSort + 1
        };
        db.BusStations.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<BusStationDto?> UpdateAsync(long id, CreateBusStationRequest req, CancellationToken ct = default)
    {
        var entity = await db.BusStations.FindAsync([id], ct);
        if (entity is null) return null;

        entity.Name = req.Name;
        entity.BusStopName = req.BusStopName;
        entity.BusRouteNumber = req.BusRouteNumber;
        entity.GbisStationId = req.GbisStationId;
        entity.GbisRouteId = req.GbisRouteId;
        entity.GbisStaOrder = req.GbisStaOrder;
        entity.Direction = req.Direction;
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.BusStations.FindAsync([id], ct);
        if (entity is null) return false;

        // FK 해제: 이 정류장을 참조하는 설정의 BusStationId를 null로
        var refs = await db.BusCommuteSettings
            .Where(s => s.BusStationId == id)
            .ToListAsync(ct);
        foreach (var r in refs) r.BusStationId = null;

        db.BusStations.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
