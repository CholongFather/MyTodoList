using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevTodoList.Shared.Constants;

namespace DevTodoList.Api.Data.Entities;

/// <summary>배포 환경 엔티티 (설정에서 관리)</summary>
[Table("Environments")]
public class EnvironmentEntity : EntityBase
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Color { get; set; } = AppColors.DefaultProject;

    public int SortOrder { get; set; }
}
