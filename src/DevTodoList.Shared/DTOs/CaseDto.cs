using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs;

/// <summary>케이스 응답 DTO</summary>
public class CaseDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CaseStatus CaseStatus { get; set; }
    public CaseCategory CaseCategory { get; set; }
    public long? EnvironmentId { get; set; }
    public string? EnvironmentName { get; set; }
    public string? EnvironmentColor { get; set; }
    public int Priority { get; set; }
    public string? Reporter { get; set; }
    public string? Assignee { get; set; }
    public string? JiraUrl { get; set; }
    public string? WideUrl { get; set; }
    public long? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string? ProjectColor { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<CaseNoteDto> Notes { get; set; } = [];
    public int NoteCount { get; set; }
    public List<CaseLinkDto> Links { get; set; } = [];
}
