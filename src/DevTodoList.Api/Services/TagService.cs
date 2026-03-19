using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>태그 서비스</summary>
public class TagService(AppDbContext db)
{
    public async Task<List<TagDto>> GetAllAsync(CancellationToken ct = default)
        => await db.Tags.OrderBy(t => t.Name).Select(t => t.ToDto()).ToListAsync(ct);

    public async Task<TagDto> CreateAsync(CreateTagRequest req, CancellationToken ct = default)
    {
        var entity = new TagEntity { Name = req.Name, Color = req.Color };
        db.Tags.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<TagDto?> UpdateAsync(long id, CreateTagRequest req, CancellationToken ct = default)
    {
        var entity = await db.Tags.FindAsync([id], ct);
        if (entity is null) return null;
        entity.Name = req.Name;
        entity.Color = req.Color;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Tags.FindAsync([id], ct);
        if (entity is null) return false;
        db.Tags.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
