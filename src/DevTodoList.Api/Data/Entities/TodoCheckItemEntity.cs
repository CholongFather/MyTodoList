using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevTodoList.Api.Data.Entities;

/// <summary>TODO 체크리스트 항목</summary>
[Table("TodoCheckItems")]
public class TodoCheckItemEntity : EntityBase
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
    public int SortOrder { get; set; }

    public long TodoItemId { get; set; }

    [ForeignKey(nameof(TodoItemId))]
    public TodoItemEntity TodoItem { get; set; } = null!;
}
