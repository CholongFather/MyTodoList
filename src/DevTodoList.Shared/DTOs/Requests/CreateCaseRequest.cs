using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs.Requests;

/// <summary>케이스 생성/수정 요청</summary>
public class CreateCaseRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CaseCategory CaseCategory { get; set; }
    public long? EnvironmentId { get; set; }
    public int Priority { get; set; }
    public string? Reporter { get; set; }
    public string? Assignee { get; set; }
    public string? JiraUrl { get; set; }
    public string? WideUrl { get; set; }
    public long? ProjectId { get; set; }
}
