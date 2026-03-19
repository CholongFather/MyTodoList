namespace DevTodoList.Shared.DTOs;

public class AssigneeTypeDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#9E9E9E";
    public bool IsMine { get; set; }
    public int SortOrder { get; set; }
}
