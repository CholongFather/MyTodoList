namespace DevTodoList.Shared.DTOs;

/// <summary>노트 유형 DTO</summary>
public class NoteTypeDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#1976D2";
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
}
