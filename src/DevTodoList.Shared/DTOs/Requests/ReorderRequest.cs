namespace DevTodoList.Shared.DTOs.Requests;

public class ReorderRequest
{
    /// <summary>순서대로 정렬된 ID 목록</summary>
    public List<long> OrderedIds { get; set; } = [];
}
