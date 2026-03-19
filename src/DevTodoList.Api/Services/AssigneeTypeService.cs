using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>담당 유형 관리 서비스</summary>
public class AssigneeTypeService(AppDbContext db)
{
    public async Task<List<AssigneeTypeDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await db.AssigneeTypes
            .OrderBy(e => e.SortOrder)
            .Select(e => new AssigneeTypeDto
            {
                Id = e.Id,
                Name = e.Name,
                Color = e.Color,
                IsMine = e.IsMine,
                SortOrder = e.SortOrder
            })
            .ToListAsync(ct);
    }

    public async Task<AssigneeTypeDto> CreateAsync(CreateAssigneeTypeRequest req, CancellationToken ct = default)
    {
        var maxOrder = await db.AssigneeTypes.MaxAsync(e => (int?)e.SortOrder, ct) ?? 0;
        var entity = new AssigneeTypeEntity
        {
            Name = req.Name,
            Color = req.Color,
            IsMine = req.IsMine,
            SortOrder = maxOrder + 1
        };
        db.AssigneeTypes.Add(entity);
        await db.SaveChangesAsync(ct);
        return new AssigneeTypeDto { Id = entity.Id, Name = entity.Name, Color = entity.Color, IsMine = entity.IsMine, SortOrder = entity.SortOrder };
    }

    public async Task<AssigneeTypeDto?> UpdateAsync(long id, CreateAssigneeTypeRequest req, CancellationToken ct = default)
    {
        var entity = await db.AssigneeTypes.FindAsync([id], ct);
        if (entity is null) return null;
        entity.Name = req.Name;
        entity.Color = req.Color;
        entity.IsMine = req.IsMine;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return new AssigneeTypeDto { Id = entity.Id, Name = entity.Name, Color = entity.Color, IsMine = entity.IsMine, SortOrder = entity.SortOrder };
    }

    public async Task ReorderAsync(List<long> orderedIds, CancellationToken ct = default)
    {
        for (var i = 0; i < orderedIds.Count; i++)
        {
            await db.AssigneeTypes.Where(e => e.Id == orderedIds[i])
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.SortOrder, i), ct);
        }
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.AssigneeTypes.FindAsync([id], ct);
        if (entity is null) return false;

        // 참조 중인 TodoItem의 FK를 null로 설정
        await db.TodoItems.Where(t => t.AssigneeTypeId == id)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.AssigneeTypeId, (long?)null), ct);

        db.AssigneeTypes.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
