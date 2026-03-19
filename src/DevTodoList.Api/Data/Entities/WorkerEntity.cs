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

    /// <summary>true면 "나" (이 작업자가 포함된 할일 → 내 작업 자동 설정)</summary>
    public bool IsMe { get; set; }

    public int SortOrder { get; set; }

    public ICollection<TodoWorkerEntity> TodoWorkers { get; set; } = [];
}
