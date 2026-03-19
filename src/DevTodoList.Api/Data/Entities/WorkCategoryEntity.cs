using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevTodoList.Api.Data.Entities;

/// <summary>작업 분류 엔티티 (팀 하위, 설정에서 관리)</summary>
[Table("WorkCategories")]
public class WorkCategoryEntity : EntityBase
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public long TeamId { get; set; }

    [ForeignKey(nameof(TeamId))]
    public TeamEntity Team { get; set; } = null!;
}
