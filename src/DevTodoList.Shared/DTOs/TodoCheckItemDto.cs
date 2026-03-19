namespace DevTodoList.Shared.DTOs;

public class TodoCheckItemDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int SortOrder { get; set; }
    public long TodoItemId { get; set; }
}
