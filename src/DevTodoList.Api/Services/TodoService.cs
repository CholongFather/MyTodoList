using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.Constants;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using DevTodoList.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>TODO 서비스</summary>
public class TodoService(AppDbContext db)
{
    private IQueryable<TodoItemEntity> BaseQuery() => db.TodoItems
        .Include(x => x.TodoTags).ThenInclude(x => x.Tag)
        .Include(x => x.CheckItems)
        .Include(x => x.Links)
        .Include(x => x.Project)
        .Include(x => x.Team)
        .Include(x => x.WorkCategory)
        .Include(x => x.AssigneeTypeEntity);

    public async Task<List<TodoItemDto>> GetFilteredAsync(
        long? projectId, int? status, string? tagIds,
        int? priority, DateTime? fromDate, DateTime? toDate,
        string? search, long? teamId, int? assigneeType, long? workCategoryId,
        long? assigneeTypeId = null,
        CancellationToken ct = default)
    {
        var query = BaseQuery();

        if (projectId.HasValue) query = query.Where(x => x.ProjectId == projectId);
        if (status.HasValue) query = query.Where(x => x.Status == status);
        if (priority.HasValue) query = query.Where(x => x.Priority == priority);
        if (fromDate.HasValue) query = query.Where(x => x.DueDate >= fromDate);
        if (toDate.HasValue) query = query.Where(x => x.DueDate <= toDate);
        if (!string.IsNullOrEmpty(search))
            query = query.Where(x => x.Title.Contains(search) || (x.Description != null && x.Description.Contains(search)));
        if (teamId.HasValue) query = query.Where(x => x.TeamId == teamId);
        if (assigneeType.HasValue) query = query.Where(x => x.AssigneeType == assigneeType);
        if (workCategoryId.HasValue) query = query.Where(x => x.WorkCategoryId == workCategoryId);
        if (assigneeTypeId.HasValue) query = query.Where(x => x.AssigneeTypeId == assigneeTypeId);
        if (!string.IsNullOrEmpty(tagIds))
        {
            var ids = tagIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => long.TryParse(x.Trim(), out var v) ? v : (long?)null)
                .Where(x => x.HasValue).Select(x => x!.Value).ToList();
            query = query.Where(x => x.TodoTags.Any(t => ids.Contains(t.TagId)));
        }

        var entities = await query
            .OrderBy(x => x.SortOrder)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

        return entities.ToDtoList();
    }

    /// <summary>페이지네이션 조회</summary>
    public async Task<PagedResult<TodoItemDto>> GetPagedAsync(
        long? projectId, int? status, string? tagIds,
        int? priority, DateTime? fromDate, DateTime? toDate,
        string? search, long? teamId, int? assigneeType, long? workCategoryId,
        int page = PaginationDefaults.Page, int pageSize = PaginationDefaults.PageSize,
        long? assigneeTypeId = null,
        CancellationToken ct = default)
    {
        var query = BaseQuery();

        if (projectId.HasValue) query = query.Where(x => x.ProjectId == projectId);
        if (status.HasValue) query = query.Where(x => x.Status == status);
        if (priority.HasValue) query = query.Where(x => x.Priority == priority);
        if (fromDate.HasValue) query = query.Where(x => x.DueDate >= fromDate);
        if (toDate.HasValue) query = query.Where(x => x.DueDate <= toDate);
        if (!string.IsNullOrEmpty(search))
            query = query.Where(x => x.Title.Contains(search) || (x.Description != null && x.Description.Contains(search)));
        if (teamId.HasValue) query = query.Where(x => x.TeamId == teamId);
        if (assigneeType.HasValue) query = query.Where(x => x.AssigneeType == assigneeType);
        if (workCategoryId.HasValue) query = query.Where(x => x.WorkCategoryId == workCategoryId);
        if (assigneeTypeId.HasValue) query = query.Where(x => x.AssigneeTypeId == assigneeTypeId);
        if (!string.IsNullOrEmpty(tagIds))
        {
            var ids = tagIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => long.TryParse(x.Trim(), out var v) ? v : (long?)null)
                .Where(x => x.HasValue).Select(x => x!.Value).ToList();
            query = query.Where(x => x.TodoTags.Any(t => ids.Contains(t.TagId)));
        }

        var totalCount = await query.CountAsync(ct);

        var entities = await query
            .OrderBy(x => x.SortOrder)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<TodoItemDto>
        {
            Items = entities.ToDtoList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<TodoItemDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await BaseQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity?.ToDto();
    }

    public async Task<TodoItemDto> CreateAsync(CreateTodoRequest req, CancellationToken ct = default)
    {
        var maxOrder = await db.TodoItems
            .Where(x => x.ProjectId == req.ProjectId)
            .MaxAsync(x => (int?)x.SortOrder, ct) ?? 0;

        // 외부 일정이면 무조건 타인 작업 강제
        if (req.IsExternal)
        {
            var othersAt = await db.AssigneeTypes.FirstOrDefaultAsync(x => !x.IsMine, ct);
            if (othersAt != null) req.AssigneeTypeId = othersAt.Id;
        }

        // AssigneeTypeId → 동작 유형 자동 결정
        var resolvedAssigneeType = (int)req.AssigneeType;
        if (req.AssigneeTypeId.HasValue)
        {
            var at = await db.AssigneeTypes.FindAsync([req.AssigneeTypeId.Value], ct);
            if (at != null) resolvedAssigneeType = at.IsMine ? 0 : 1;
        }

        var entity = new TodoItemEntity
        {
            Title = req.Title,
            Description = req.Description,
            Priority = (int)req.Priority,
            DueDate = req.DueDate,
            StartDate = req.StartDate,
            EndDate = req.EndDate,
            ProjectId = req.ProjectId,
            SortOrder = maxOrder + 1,
            TeamId = req.TeamId,
            WorkCategoryId = req.WorkCategoryId,
            AssigneeType = resolvedAssigneeType,
            AssigneeTypeId = req.AssigneeTypeId,
            IsExternal = req.IsExternal,
            ExternalLabel = req.ExternalLabel,
            PlannedStartDate = req.StartDate,
            PlannedEndDate = req.EndDate ?? req.DueDate,
        };

        db.TodoItems.Add(entity);
        await db.SaveChangesAsync(ct);

        // 태그 연결
        if (req.TagIds.Count > 0)
        {
            foreach (var tagId in req.TagIds)
                db.TodoTags.Add(new TodoTagEntity { TodoItemId = entity.Id, TagId = tagId });
            await db.SaveChangesAsync(ct);
        }

        return (await GetByIdAsync(entity.Id, ct))!;
    }

    public async Task<TodoItemDto?> UpdateAsync(long id, UpdateTodoRequest req, CancellationToken ct = default)
    {
        var entity = await db.TodoItems.Include(x => x.TodoTags).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return null;

        var oldStatus = entity.Status;
        entity.Title = req.Title;
        entity.Description = req.Description;
        entity.Status = (int)req.Status;
        entity.Priority = (int)req.Priority;
        entity.DueDate = req.DueDate;
        entity.StartDate = req.StartDate;
        entity.EndDate = req.EndDate;
        entity.ProjectId = req.ProjectId;
        entity.TeamId = req.TeamId;
        entity.WorkCategoryId = req.WorkCategoryId;
        // 외부 일정이면 무조건 타인 작업 강제
        if (req.IsExternal)
        {
            var othersAt = await db.AssigneeTypes.FirstOrDefaultAsync(x => !x.IsMine, ct);
            if (othersAt != null) req.AssigneeTypeId = othersAt.Id;
        }
        entity.AssigneeTypeId = req.AssigneeTypeId;
        // AssigneeTypeId → 동작 유형 자동 결정
        if (req.AssigneeTypeId.HasValue)
        {
            var at = await db.AssigneeTypes.FindAsync([req.AssigneeTypeId.Value], ct);
            entity.AssigneeType = at?.IsMine == true ? 0 : 1;
        }
        else
        {
            entity.AssigneeType = (int)req.AssigneeType;
        }
        entity.IsExternal = req.IsExternal;
        entity.ExternalLabel = req.ExternalLabel;
        entity.UpdatedAt = DateTime.UtcNow;

        // 실제 시작일 자동 설정: → InProgress 전환 시
        if (req.Status == TodoStatus.InProgress && oldStatus != (int)TodoStatus.InProgress)
            entity.ActualStartDate ??= DateTime.UtcNow;

        // 완료 시각 + 실제 종료일 처리
        if (req.Status == TodoStatus.Done && oldStatus != (int)TodoStatus.Done)
        {
            entity.CompletedAt = DateTime.UtcNow;
            entity.ActualEndDate = DateTime.UtcNow;
        }
        else if (req.Status != TodoStatus.Done)
        {
            entity.CompletedAt = null;
            entity.ActualEndDate = null;
        }

        // 태그 갱신
        db.TodoTags.RemoveRange(entity.TodoTags);
        foreach (var tagId in req.TagIds)
            db.TodoTags.Add(new TodoTagEntity { TodoItemId = id, TagId = tagId });

        await db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> UpdateStatusAsync(long id, TodoStatus newStatus, CancellationToken ct = default)
    {
        var entity = await db.TodoItems.FindAsync([id], ct);
        if (entity is null) return false;

        var oldStatus = (TodoStatus)entity.Status;
        entity.Status = (int)newStatus;
        entity.UpdatedAt = DateTime.UtcNow;

        // 실제 시작일 자동 설정: → InProgress 전환 시
        if (newStatus == TodoStatus.InProgress && oldStatus != TodoStatus.InProgress)
            entity.ActualStartDate ??= DateTime.UtcNow;

        // 완료 시각 + 실제 종료일
        if (newStatus == TodoStatus.Done && entity.CompletedAt is null)
        {
            entity.CompletedAt = DateTime.UtcNow;
            entity.ActualEndDate = DateTime.UtcNow;
        }
        else if (newStatus != TodoStatus.Done)
        {
            entity.CompletedAt = null;
            entity.ActualEndDate = null;
        }

        await db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>태그 즉시 동기화</summary>
    public async Task<bool> UpdateTagsAsync(long id, List<long> tagIds, CancellationToken ct = default)
    {
        var entity = await db.TodoItems.Include(x => x.TodoTags).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;
        db.TodoTags.RemoveRange(entity.TodoTags);
        foreach (var tagId in tagIds)
            db.TodoTags.Add(new TodoTagEntity { TodoItemId = id, TagId = tagId });
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>담당 유형 즉시 변경</summary>
    public async Task<bool> UpdateAssigneeTypeAsync(long id, int assigneeType, long? assigneeTypeId = null, CancellationToken ct = default)
    {
        var entity = await db.TodoItems.FindAsync([id], ct);
        if (entity is null) return false;
        entity.AssigneeTypeId = assigneeTypeId;
        if (assigneeTypeId.HasValue)
        {
            var at = await db.AssigneeTypes.FindAsync([assigneeTypeId.Value], ct);
            entity.AssigneeType = at?.IsMine == true ? 0 : 1;
        }
        else
        {
            entity.AssigneeType = assigneeType;
        }
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.TodoItems.FindAsync([id], ct);
        if (entity is null) return false;
        db.TodoItems.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>할일 복제 (태그 포함, 상태는 Todo로 초기화)</summary>
    public async Task<TodoItemDto?> DuplicateAsync(long id, CancellationToken ct = default)
    {
        var source = await BaseQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (source is null) return null;

        var maxOrder = await db.TodoItems
            .Where(x => x.ProjectId == source.ProjectId)
            .MaxAsync(x => (int?)x.SortOrder, ct) ?? 0;

        var copy = new TodoItemEntity
        {
            Title = $"{source.Title} (복사)",
            Description = source.Description,
            Priority = source.Priority,
            DueDate = source.DueDate,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            ProjectId = source.ProjectId,
            SortOrder = maxOrder + 1,
            TeamId = source.TeamId,
            WorkCategoryId = source.WorkCategoryId,
            AssigneeType = source.AssigneeType,
            AssigneeTypeId = source.AssigneeTypeId,
            IsExternal = source.IsExternal,
            ExternalLabel = source.ExternalLabel,
            PlannedStartDate = source.StartDate,
            PlannedEndDate = source.EndDate ?? source.DueDate,
        };
        db.TodoItems.Add(copy);
        await db.SaveChangesAsync(ct);

        // 태그 복제
        if (source.TodoTags?.Count > 0)
        {
            foreach (var tag in source.TodoTags)
                db.TodoTags.Add(new TodoTagEntity { TodoItemId = copy.Id, TagId = tag.TagId });
            await db.SaveChangesAsync(ct);
        }

        // 체크리스트 복제
        if (source.CheckItems?.Count > 0)
        {
            foreach (var check in source.CheckItems)
                db.TodoCheckItems.Add(new TodoCheckItemEntity { Title = check.Title, TodoItemId = copy.Id, SortOrder = check.SortOrder });
            await db.SaveChangesAsync(ct);
        }

        return (await GetByIdAsync(copy.Id, ct))!;
    }

    public async Task ReorderAsync(ReorderRequest req, CancellationToken ct = default)
    {
        var entities = await db.TodoItems
            .Where(x => req.OrderedIds.Contains(x.Id))
            .ToListAsync(ct);
        var map = entities.ToDictionary(x => x.Id);
        for (int i = 0; i < req.OrderedIds.Count; i++)
        {
            if (map.TryGetValue(req.OrderedIds[i], out var entity))
            {
                entity.SortOrder = i;
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        await db.SaveChangesAsync(ct);
    }
}
