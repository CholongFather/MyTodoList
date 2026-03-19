using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs.Requests;

public class UpdateStatusRequest
{
    public TodoStatus Status { get; set; }
}
