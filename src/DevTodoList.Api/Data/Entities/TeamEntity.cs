using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevTodoList.Shared.Constants;

namespace DevTodoList.Api.Data.Entities;

/// <summary>팀 엔티티 (설정에서 관리)</summary>
[Table("Teams")]
public class TeamEntity : EntityBase
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Color { get; set; } = AppColors.DefaultProject;

    public int SortOrder { get; set; }

    /// <summary>내 팀 여부</summary>
    public bool IsMine { get; set; }

    public ICollection<WorkCategoryEntity> WorkCategories { get; set; } = [];
}
