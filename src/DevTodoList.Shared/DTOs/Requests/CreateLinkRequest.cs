using System.ComponentModel.DataAnnotations;
using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs.Requests;

public class CreateLinkRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    [Required]
    [StringLength(2000)]
    [Url]
    public string Url { get; set; } = string.Empty;
    public LinkType LinkType { get; set; } = LinkType.Custom;
    public long? LinkTypeId { get; set; }
}
