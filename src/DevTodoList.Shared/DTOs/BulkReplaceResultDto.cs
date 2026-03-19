namespace DevTodoList.Shared.DTOs;

/// <summary>일괄 교체 결과</summary>
public class BulkReplaceResultDto
{
    public int DeletedCount { get; set; }
    public List<TodoItemDto> CreatedItems { get; set; } = [];
}
