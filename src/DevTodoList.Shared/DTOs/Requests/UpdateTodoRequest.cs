using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs.Requests;

public class UpdateTodoRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TodoStatus Status { get; set; }
    public TodoPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public long ProjectId { get; set; }
    public List<long> TagIds { get; set; } = [];
    public List<long> WorkerIds { get; set; } = [];

    // 계획일 수정
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }

    // 팀/담당자
    public long? TeamId { get; set; }
    public long? WorkCategoryId { get; set; }
    public AssigneeType AssigneeType { get; set; }
    public long? AssigneeTypeId { get; set; }

    // 외부 일정
    public bool IsExternal { get; set; }
    public string? ExternalLabel { get; set; }
}
