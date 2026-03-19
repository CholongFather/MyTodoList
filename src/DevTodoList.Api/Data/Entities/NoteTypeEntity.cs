using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevTodoList.Api.Data.Entities;

/// <summary>노트 유형 마스터</summary>
[Table("NoteTypes")]
public class NoteTypeEntity : EntityBase
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Color { get; set; } = "#1976D2";

    /// <summary>Material Icon 이름 (예: Comment, MenuBook, Build, FindInPage)</summary>
    [MaxLength(50)]
    public string? Icon { get; set; }

    public int SortOrder { get; set; }
}
