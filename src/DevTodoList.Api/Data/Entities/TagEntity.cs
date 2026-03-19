using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevTodoList.Shared.Constants;

namespace DevTodoList.Api.Data.Entities;

/// <summary>태그 엔티티</summary>
[Table("Tags")]
public class TagEntity : EntityBase
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Color { get; set; } = AppColors.DefaultTag;

    public ICollection<TodoTagEntity> TodoTags { get; set; } = [];
}
