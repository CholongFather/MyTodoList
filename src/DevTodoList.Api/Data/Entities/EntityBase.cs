using System.ComponentModel.DataAnnotations;

namespace DevTodoList.Api.Data.Entities;

/// <summary>모든 엔티티의 기본 클래스</summary>
public abstract class EntityBase
{
    [Key]
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
