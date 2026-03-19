using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>노트 유형 관리 서비스</summary>
public class NoteTypeService(AppDbContext db)
{
    public async Task<List<NoteTypeDto>> GetAllAsync(CancellationToken ct = default)
        => await db.NoteTypes.OrderBy(n => n.SortOrder).Select(n => new NoteTypeDto
        {
            Id = n.Id, Name = n.Name, Color = n.Color, Icon = n.Icon, SortOrder = n.SortOrder
        }).ToListAsync(ct);

    public async Task<NoteTypeDto> CreateAsync(CreateNoteTypeRequest req, CancellationToken ct = default)
    {
        var maxOrder = await db.NoteTypes.MaxAsync(n => (int?)n.SortOrder, ct) ?? 0;
        var entity = new NoteTypeEntity
        {
            Name = req.Name,
            Color = req.Color,
            Icon = req.Icon,
            SortOrder = maxOrder + 1,
            CreatedAt = DateTime.UtcNow
        };
        db.NoteTypes.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<NoteTypeDto?> UpdateAsync(long id, CreateNoteTypeRequest req, CancellationToken ct = default)
    {
        var entity = await db.NoteTypes.FindAsync([id], ct);
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
        var entity = await db.NoteTypes.FindAsync([id], ct);
        if (entity is null) return false;
        // FK null 처리
        await db.CaseNotes.Where(n => n.NoteTypeId == id).ForEachAsync(n => n.NoteTypeId = null, ct);
        db.NoteTypes.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
