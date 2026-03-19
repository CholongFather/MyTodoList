using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>체크리스트 서비스</summary>
public class CheckItemService(AppDbContext db)
{
    public async Task<List<TodoCheckItemDto>> GetByTodoIdAsync(long todoId, CancellationToken ct = default)
        => await db.TodoCheckItems
            .Where(c => c.TodoItemId == todoId)
            .OrderBy(c => c.SortOrder)
            .Select(c => c.ToDto())
            .ToListAsync(ct);

    public async Task<TodoCheckItemDto> CreateAsync(long todoId, CreateCheckItemRequest req, CancellationToken ct = default)
    {
        var maxOrder = await db.TodoCheckItems
            .Where(c => c.TodoItemId == todoId)
            .MaxAsync(c => (int?)c.SortOrder, ct) ?? 0;

        var entity = new TodoCheckItemEntity
        {
            Title = req.Title,
            TodoItemId = todoId,
            SortOrder = maxOrder + 1
        };
        db.TodoCheckItems.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<TodoCheckItemDto?> UpdateAsync(long id, CreateCheckItemRequest req, CancellationToken ct = default)
    {
        var entity = await db.TodoCheckItems.FindAsync([id], ct);
        if (entity is null) return null;
        entity.Title = req.Title;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<bool> ToggleAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.TodoCheckItems.FindAsync([id], ct);
        if (entity is null) return false;
        entity.IsCompleted = !entity.IsCompleted;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.TodoCheckItems.FindAsync([id], ct);
        if (entity is null) return false;
        db.TodoCheckItems.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task ReorderAsync(long todoId, ReorderRequest req, CancellationToken ct = default)
    {
        for (int i = 0; i < req.OrderedIds.Count; i++)
        {
            var entity = await db.TodoCheckItems.FindAsync([req.OrderedIds[i]], ct);
            if (entity is not null && entity.TodoItemId == todoId)
            {
                entity.SortOrder = i;
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        await db.SaveChangesAsync(ct);
    }
}
