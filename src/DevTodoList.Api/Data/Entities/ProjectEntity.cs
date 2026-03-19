using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevTodoList.Shared.Constants;

namespace DevTodoList.Api.Data.Entities;

/// <summary>프로젝트 엔티티</summary>
[Table("Projects")]
public class ProjectEntity : EntityBase
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>프로젝트 색상 (hex)</summary>
    [MaxLength(10)]
    public string Color { get; set; } = AppColors.DefaultProject;

    public int SortOrder { get; set; }
    public bool IsArchived { get; set; }

    public ICollection<TodoItemEntity> TodoItems { get; set; } = [];
    public ICollection<ProjectMetaEntity> Metas { get; set; } = [];
}
