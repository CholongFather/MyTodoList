using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs.Requests;

public class UpdateProjectStatusRequest
{
    public ProjectStatus Status { get; set; }
}
