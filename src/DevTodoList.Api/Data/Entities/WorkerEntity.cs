using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevTodoList.Api.Data.Entities;

/// <summary>작업자 마스터</summary>
[Table("Workers")]
public class WorkerEntity : EntityBase
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Color { get; set; } = "#9E9E9E";

    public int SortOrder { get; set; }

    public ICollection<TodoWorkerEntity> TodoWorkers { get; set; } = [];
}
