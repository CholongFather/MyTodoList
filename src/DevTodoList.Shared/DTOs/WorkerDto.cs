namespace DevTodoList.Shared.DTOs;

/// <summary>작업자 DTO</summary>
public class WorkerDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#9E9E9E";
    public int SortOrder { get; set; }
}
