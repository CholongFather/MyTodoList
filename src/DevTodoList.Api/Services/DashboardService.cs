using DevTodoList.Api.Data;
using DevTodoList.Shared.Constants;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>대시보드 서비스</summary>
public class DashboardService(AppDbContext db)
{
    public async Task<DashboardDto> GetAsync(CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        var weekLater = today.AddDays(NotificationDefaults.UpcomingDays);

        var allActive = await db.TodoItems
            .AsNoTracking()
            .Include(x => x.TodoTags).ThenInclude(x => x.Tag)
            .Include(x => x.CheckItems)
            .Include(x => x.Project)
            .Include(x => x.Team)
            .Where(x => x.Status != (int)TodoStatus.Archived)
            .ToListAsync(ct);

        var todayItems = allActive
            .Where(x => x.DueDate?.Date == today && x.Status != (int)TodoStatus.Done)
            .ToList();

        var overdueItems = allActive
            .Where(x => x.DueDate < today && x.Status != (int)TodoStatus.Done)
            .ToList();

        var upcomingItems = allActive
            .Where(x => x.DueDate > today && x.DueDate <= weekLater && x.Status != (int)TodoStatus.Done)
            .ToList();

        var statusCounts = allActive
            .GroupBy(x => x.Status)
            .ToDictionary(g => ((TodoStatus)g.Key).ToString(), g => g.Count());

        var doneCount = allActive.Count(x => x.Status == (int)TodoStatus.Done);

        return new DashboardDto
        {
            TodayTodos = todayItems.ToDtoList(),
            OverdueTodos = overdueItems.ToDtoList(),
            UpcomingTodos = upcomingItems.ToDtoList(),
            StatusCounts = statusCounts,
            TotalCount = allActive.Count,
            CompletionRate = allActive.Count > 0
                ? Math.Round(doneCount * 100.0 / allActive.Count, 1)
                : 0,
            // 내 작업 / 타인 작업 분리
            MyTodayTodos = allActive.Where(x => x.AssigneeType == (int)AssigneeType.Mine && x.DueDate?.Date == today && x.Status != (int)TodoStatus.Done).ToList().ToDtoList(),
            MyOverdueTodos = allActive.Where(x => x.AssigneeType == (int)AssigneeType.Mine && x.DueDate < today && x.Status != (int)TodoStatus.Done).ToList().ToDtoList(),
            MyUpcomingTodos = allActive.Where(x => x.AssigneeType == (int)AssigneeType.Mine && x.DueDate > today && x.DueDate <= weekLater && x.Status != (int)TodoStatus.Done).ToList().ToDtoList(),
            OthersOverdueTodos = allActive.Where(x => x.AssigneeType == (int)AssigneeType.Others && x.DueDate < today && x.Status != (int)TodoStatus.Done).ToList().ToDtoList(),
            OthersOverdueCount = allActive.Count(x => x.AssigneeType == (int)AssigneeType.Others && x.DueDate < today && x.Status != (int)TodoStatus.Done),
            TeamCounts = allActive.Where(x => x.TeamId != null).GroupBy(x => x.Team?.Name ?? AppLabels.UnassignedTeam).ToDictionary(g => g.Key, g => g.Count()),
        };
    }
}
