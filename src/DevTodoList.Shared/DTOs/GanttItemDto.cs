using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs;

public class GanttItemDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TodoStatus Status { get; set; }
    public TodoPriority Priority { get; set; }
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectColor { get; set; } = string.Empty;
    /// <summary>진행률 (체크리스트 기반 0~100)</summary>
    public double ProgressPercent { get; set; }

    // 계획 vs 실제 일정
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }

    // 팀/담당자
    public long? TeamId { get; set; }
    public string? TeamName { get; set; }
    public long? WorkCategoryId { get; set; }
    public string? WorkCategoryName { get; set; }
    public AssigneeType AssigneeType { get; set; }
    public long? AssigneeTypeId { get; set; }
    public string? AssigneeTypeName { get; set; }
    public string? AssigneeTypeColor { get; set; }

    // 외부 일정
    public bool IsExternal { get; set; }
    public string? ExternalLabel { get; set; }

    // 작업자
    public List<WorkerDto> Workers { get; set; } = [];

    /// <summary>지연 여부</summary>
    public bool IsDelayed { get; set; }
    /// <summary>지연 일수</summary>
    public int DelayDays { get; set; }
}
