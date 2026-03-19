namespace DevTodoList.Shared.DTOs;

public class DashboardDto
{
    public List<TodoItemDto> TodayTodos { get; set; } = [];
    public List<TodoItemDto> OverdueTodos { get; set; } = [];
    public List<TodoItemDto> UpcomingTodos { get; set; } = [];
    public Dictionary<string, int> StatusCounts { get; set; } = new();
    public int TotalCount { get; set; }
    public double CompletionRate { get; set; }

    // 내 작업 / 타인 작업 분리
    public List<TodoItemDto> MyTodayTodos { get; set; } = [];
    public List<TodoItemDto> MyOverdueTodos { get; set; } = [];
    public List<TodoItemDto> MyUpcomingTodos { get; set; } = [];
    public List<TodoItemDto> OthersOverdueTodos { get; set; } = [];
    public int OthersOverdueCount { get; set; }

    // 팀별 통계
    public Dictionary<string, int> TeamCounts { get; set; } = new();
}
