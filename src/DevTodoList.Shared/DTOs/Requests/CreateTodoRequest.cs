using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs.Requests;

public class CreateTodoRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TodoPriority Priority { get; set; } = TodoPriority.Normal;
    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public long ProjectId { get; set; }
    public List<long> TagIds { get; set; } = [];
    public List<long> WorkerIds { get; set; } = [];

    // 팀/담당자
    public long? TeamId { get; set; }
    public long? WorkCategoryId { get; set; }
    public AssigneeType AssigneeType { get; set; } = AssigneeType.Mine;
    public long? AssigneeTypeId { get; set; }

    // 외부 일정
    public bool IsExternal { get; set; }
    public string? ExternalLabel { get; set; }
}
