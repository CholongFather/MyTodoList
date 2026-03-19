using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>작업자 관리 서비스</summary>
public class WorkerService(AppDbContext db)
{
    public async Task<List<WorkerDto>> GetAllAsync(CancellationToken ct = default)
        => await db.Workers.OrderBy(w => w.SortOrder).Select(w => new WorkerDto
        {
            Id = w.Id, Name = w.Name, Color = w.Color, IsMe = w.IsMe, SortOrder = w.SortOrder
        }).ToListAsync(ct);

    public async Task<WorkerDto> CreateAsync(CreateWorkerRequest req, CancellationToken ct = default)
    {
        var maxOrder = await db.Workers.MaxAsync(w => (int?)w.SortOrder, ct) ?? 0;
        var entity = new WorkerEntity
        {
            Name = req.Name,
            Color = req.Color,
            IsMe = req.IsMe,
            SortOrder = maxOrder + 1,
            CreatedAt = DateTime.UtcNow
        };
        db.Workers.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<WorkerDto?> UpdateAsync(long id, CreateWorkerRequest req, CancellationToken ct = default)
    {
        var entity = await db.Workers.FindAsync([id], ct);
        if (entity is null) return null;
        entity.Name = req.Name;
        entity.Color = req.Color;
        entity.IsMe = req.IsMe;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<bool> ReorderAsync(List<long> orderedIds, CancellationToken ct = default)
    {
        var entities = await db.Workers.Where(w => orderedIds.Contains(w.Id)).ToListAsync(ct);
        for (var i = 0; i < orderedIds.Count; i++)
        {
            var e = entities.FirstOrDefault(x => x.Id == orderedIds[i]);
            if (e != null) e.SortOrder = i + 1;
        }
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Workers.FindAsync([id], ct);
        if (entity is null) return false;
        // 연관 TodoWorker 삭제
        var refs = await db.TodoWorkers.Where(tw => tw.WorkerId == id).ToListAsync(ct);
        db.TodoWorkers.RemoveRange(refs);
        db.Workers.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
