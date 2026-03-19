using DevTodoList.Api.Data;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>간트차트 서비스</summary>
public class GanttService(AppDbContext db)
{
    public async Task<List<GanttItemDto>> GetAsync(
        long? projectId, DateTime? fromDate, DateTime? toDate,
        long? teamId, int? assigneeType,
        CancellationToken ct = default)
    {
        var query = db.TodoItems
            .AsNoTracking()
            .Include(x => x.CheckItems)
            .Include(x => x.Project)
            .Include(x => x.Team)
            .Include(x => x.WorkCategory)
            .Include(x => x.AssigneeTypeEntity)
            .Where(x => x.Status != (int)TodoStatus.Archived)
            .Where(x => x.StartDate != null || x.EndDate != null || x.DueDate != null);

        if (projectId.HasValue)
            query = query.Where(x => x.ProjectId == projectId);
        if (teamId.HasValue)
            query = query.Where(x => x.TeamId == teamId);
        if (assigneeType.HasValue)
            query = query.Where(x => x.AssigneeType == assigneeType);

        var items = await query.OrderBy(x => x.StartDate ?? x.CreatedAt).ToListAsync(ct);

        var result = items.Select(x => x.ToGanttDto()).ToList();

        // 날짜 범위 필터
        if (fromDate.HasValue)
            result = result.Where(x => x.EndDate >= fromDate.Value).ToList();
        if (toDate.HasValue)
            result = result.Where(x => x.StartDate <= toDate.Value).ToList();

        return result;
    }
}
