using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevTodoList.Api.Data.Entities;

/// <summary>담당 유형 엔티티 (설정에서 관리)</summary>
[Table("AssigneeTypes")]
public class AssigneeTypeEntity : EntityBase
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Color { get; set; } = "#9E9E9E";

    /// <summary>내 작업 동작 여부 (true=Mine 동작, false=Others 동작)</summary>
    public bool IsMine { get; set; }

    public int SortOrder { get; set; }
}
