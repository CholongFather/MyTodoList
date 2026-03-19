using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs;

public class TodoItemDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TodoStatus Status { get; set; }
    public TodoPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int SortOrder { get; set; }

    // 팀/담당자
    public long? TeamId { get; set; }
    public string? TeamName { get; set; }
    public string? TeamColor { get; set; }
    public long? WorkCategoryId { get; set; }
    public string? WorkCategoryName { get; set; }
    public AssigneeType AssigneeType { get; set; }
    public long? AssigneeTypeId { get; set; }
    public string? AssigneeTypeName { get; set; }
    public string? AssigneeTypeColor { get; set; }

    // 외부 일정
    public bool IsExternal { get; set; }
    public string? ExternalLabel { get; set; }

    // 계획 vs 실제 일정
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }

    /// <summary>지연 여부</summary>
    public bool IsDelayed => ActualEndDate.HasValue && PlannedEndDate.HasValue
        && ActualEndDate.Value > PlannedEndDate.Value;
    /// <summary>지연 일수</summary>
    public int DelayDays => IsDelayed
        ? (ActualEndDate!.Value - PlannedEndDate!.Value).Days : 0;

    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectColor { get; set; } = string.Empty;
    public List<TagDto> Tags { get; set; } = [];
    public List<WorkerDto> Workers { get; set; } = [];
    public List<TodoCheckItemDto> CheckItems { get; set; } = [];
    public List<TodoLinkDto> Links { get; set; } = [];
    public int CheckItemTotal { get; set; }
    public int CheckItemCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}
