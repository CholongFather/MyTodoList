using DevTodoList.Shared.Constants;

namespace DevTodoList.Shared.DTOs.Requests;

public class CreateTagRequest
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = AppColors.DefaultTag;
}
