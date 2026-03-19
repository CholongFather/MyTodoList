using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>링크 서비스</summary>
public class LinkService(AppDbContext db)
{
    public async Task<List<TodoLinkDto>> GetByTodoIdAsync(long todoId, CancellationToken ct = default)
        => await db.TodoLinks
            .Where(l => l.TodoItemId == todoId)
            .Select(l => l.ToDto())
            .ToListAsync(ct);

    public async Task<TodoLinkDto> CreateAsync(long todoId, CreateLinkRequest req, CancellationToken ct = default)
    {
        // FK 검증: LinkTypeId가 지정된 경우 존재 확인
        if (req.LinkTypeId.HasValue && !await db.LinkTypes.AnyAsync(l => l.Id == req.LinkTypeId.Value, ct))
            throw new InvalidOperationException($"링크 유형 ID {req.LinkTypeId}이(가) 존재하지 않습니다.");

        var entity = new TodoLinkEntity
        {
            Title = req.Title,
            Url = req.Url,
            LinkType = (int)req.LinkType,
            LinkTypeId = req.LinkTypeId,
            TodoItemId = todoId
        };
        db.TodoLinks.Add(entity);
        await db.SaveChangesAsync(ct);
        if (entity.LinkTypeId.HasValue)
            await db.Entry(entity).Reference(l => l.LinkTypeEntity).LoadAsync(ct);
        return entity.ToDto();
    }

    public async Task<TodoLinkDto?> UpdateAsync(long id, CreateLinkRequest req, CancellationToken ct = default)
    {
        var entity = await db.TodoLinks.FindAsync([id], ct);
        if (entity is null) return null;
        entity.Title = req.Title;
        entity.Url = req.Url;
        entity.LinkType = (int)req.LinkType;
        entity.LinkTypeId = req.LinkTypeId;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        if (entity.LinkTypeId.HasValue)
            await db.Entry(entity).Reference(l => l.LinkTypeEntity).LoadAsync(ct);
        return entity.ToDto();
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.TodoLinks.FindAsync([id], ct);
        if (entity is null) return false;
        db.TodoLinks.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
