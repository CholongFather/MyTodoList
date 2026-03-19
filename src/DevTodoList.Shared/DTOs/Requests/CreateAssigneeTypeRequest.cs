namespace DevTodoList.Shared.DTOs.Requests;

public class CreateAssigneeTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#9E9E9E";
    public bool IsMine { get; set; }
}
