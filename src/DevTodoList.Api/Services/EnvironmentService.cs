using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>배포 환경 관리 서비스</summary>
public class EnvironmentService(AppDbContext db)
{
    public async Task<List<EnvironmentDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await db.Environments
            .OrderBy(e => e.SortOrder)
            .Select(e => new EnvironmentDto
            {
                Id = e.Id,
                Name = e.Name,
                Color = e.Color,
                SortOrder = e.SortOrder
            })
            .ToListAsync(ct);
    }

    public async Task<EnvironmentDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Environments.FindAsync([id], ct);
        if (entity is null) return null;
        return new EnvironmentDto { Id = entity.Id, Name = entity.Name, Color = entity.Color, SortOrder = entity.SortOrder };
    }

    public async Task<EnvironmentDto> CreateAsync(CreateEnvironmentRequest req, CancellationToken ct = default)
    {
        var maxOrder = await db.Environments.MaxAsync(e => (int?)e.SortOrder, ct) ?? 0;
        var entity = new EnvironmentEntity
        {
            Name = req.Name,
            Color = req.Color,
            SortOrder = maxOrder + 1
        };
        db.Environments.Add(entity);
        await db.SaveChangesAsync(ct);
        return new EnvironmentDto { Id = entity.Id, Name = entity.Name, Color = entity.Color, SortOrder = entity.SortOrder };
    }

    public async Task<EnvironmentDto?> UpdateAsync(long id, CreateEnvironmentRequest req, CancellationToken ct = default)
    {
        var entity = await db.Environments.FindAsync([id], ct);
        if (entity is null) return null;
        entity.Name = req.Name;
        entity.Color = req.Color;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return new EnvironmentDto { Id = entity.Id, Name = entity.Name, Color = entity.Color, SortOrder = entity.SortOrder };
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Environments.FindAsync([id], ct);
        if (entity is null) return false;
        db.Environments.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
