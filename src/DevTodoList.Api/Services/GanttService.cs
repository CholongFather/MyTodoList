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

        if (fromDate.HasValue)
            query = query.Where(x => (x.EndDate ?? x.DueDate ?? x.CreatedAt.AddDays(1)) >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(x => (x.StartDate ?? x.CreatedAt) <= toDate.Value);

        var items = await query.OrderBy(x => x.StartDate ?? x.CreatedAt).ToListAsync(ct);
        return items.Select(x => x.ToGanttDto()).ToList();
    }
}
