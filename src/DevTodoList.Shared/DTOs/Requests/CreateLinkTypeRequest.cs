using System.ComponentModel.DataAnnotations;

namespace DevTodoList.Shared.DTOs.Requests;

/// <summary>링크 유형 생성/수정 요청</summary>
public class CreateLinkTypeRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#9E9E9E";
    public string? Icon { get; set; }
}
