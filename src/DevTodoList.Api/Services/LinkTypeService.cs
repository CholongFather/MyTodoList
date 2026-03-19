using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>링크 유형 관리 서비스</summary>
public class LinkTypeService(AppDbContext db)
{
    public async Task<List<LinkTypeDto>> GetAllAsync(CancellationToken ct = default)
        => await db.LinkTypes.OrderBy(l => l.SortOrder).Select(l => new LinkTypeDto
        {
            Id = l.Id, Name = l.Name, Color = l.Color, Icon = l.Icon, SortOrder = l.SortOrder
        }).ToListAsync(ct);

    public async Task<LinkTypeDto> CreateAsync(CreateLinkTypeRequest req, CancellationToken ct = default)
    {
        var maxOrder = await db.LinkTypes.MaxAsync(l => (int?)l.SortOrder, ct) ?? 0;
        var entity = new LinkTypeEntity
        {
            Name = req.Name,
            Color = req.Color,
            Icon = req.Icon,
            SortOrder = maxOrder + 1,
            CreatedAt = DateTime.UtcNow
        };
        db.LinkTypes.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<LinkTypeDto?> UpdateAsync(long id, CreateLinkTypeRequest req, CancellationToken ct = default)
    {
        var entity = await db.LinkTypes.FindAsync([id], ct);
        if (entity is null) return null;
        entity.Name = req.Name;
        entity.Color = req.Color;
        entity.Icon = req.Icon;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.LinkTypes.FindAsync([id], ct);
        if (entity is null) return false;
        // FK null 처리
        await db.CaseLinks.Where(l => l.LinkTypeId == id).ForEachAsync(l => l.LinkTypeId = null, ct);
        await db.TodoLinks.Where(l => l.LinkTypeId == id).ForEachAsync(l => l.LinkTypeId = null, ct);
        db.LinkTypes.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
