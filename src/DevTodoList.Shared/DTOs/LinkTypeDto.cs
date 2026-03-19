namespace DevTodoList.Shared.DTOs;

/// <summary>링크 유형 DTO</summary>
public class LinkTypeDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#9E9E9E";
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
}
