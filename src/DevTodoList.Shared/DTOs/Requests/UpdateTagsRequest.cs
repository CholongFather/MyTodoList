namespace DevTodoList.Shared.DTOs.Requests;

public class UpdateTagsRequest
{
    public List<long> TagIds { get; set; } = [];
}
