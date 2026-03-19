using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs;

public class CaseLinkDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public LinkType LinkType { get; set; }
    public long? LinkTypeId { get; set; }
    public string? LinkTypeName { get; set; }
    public string? LinkTypeColor { get; set; }
    public long CaseId { get; set; }
}
