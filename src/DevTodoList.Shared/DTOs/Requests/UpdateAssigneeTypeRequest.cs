using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs.Requests;

public class UpdateAssigneeTypeRequest
{
    public AssigneeType AssigneeType { get; set; }
    public long? AssigneeTypeId { get; set; }
}
