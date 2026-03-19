using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs.Requests;

public class CreateLinkRequest
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public LinkType LinkType { get; set; } = LinkType.Custom;
    public long? LinkTypeId { get; set; }
}
