using System.ComponentModel.DataAnnotations.Schema;

namespace DevTodoList.Api.Data.Entities;

/// <summary>TODO-작업자 다대다 연결</summary>
[Table("TodoWorkers")]
public class TodoWorkerEntity
{
    public long TodoItemId { get; set; }

    [ForeignKey(nameof(TodoItemId))]
    public TodoItemEntity TodoItem { get; set; } = null!;

    public long WorkerId { get; set; }

    [ForeignKey(nameof(WorkerId))]
    public WorkerEntity Worker { get; set; } = null!;
}
