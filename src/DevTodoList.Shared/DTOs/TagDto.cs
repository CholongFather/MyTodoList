using DevTodoList.Shared.Constants;

namespace DevTodoList.Shared.DTOs;

public class TagDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = AppColors.DefaultTag;
}
