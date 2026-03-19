using System.ComponentModel.DataAnnotations;

namespace DevTodoList.Shared.DTOs.Requests;

/// <summary>노트 유형 생성/수정 요청</summary>
public class CreateNoteTypeRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#1976D2";
    public string? Icon { get; set; }
}
