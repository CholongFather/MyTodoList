namespace DevTodoList.Shared.DTOs;

/// <summary>작업 분류 DTO</summary>
public class WorkCategoryDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public long TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
}
