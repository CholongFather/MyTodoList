using System.ComponentModel.DataAnnotations.Schema;

namespace DevTodoList.Api.Data.Entities;

/// <summary>TODO-태그 다대다 연결</summary>
[Table("TodoTags")]
public class TodoTagEntity
{
    public long TodoItemId { get; set; }

    [ForeignKey(nameof(TodoItemId))]
    public TodoItemEntity TodoItem { get; set; } = null!;

    public long TagId { get; set; }

    [ForeignKey(nameof(TagId))]
    public TagEntity Tag { get; set; } = null!;
}
