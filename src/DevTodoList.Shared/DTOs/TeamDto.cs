using DevTodoList.Shared.Constants;

namespace DevTodoList.Shared.DTOs;

/// <summary>팀 DTO</summary>
public class TeamDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = AppColors.DefaultProject;
    public int SortOrder { get; set; }
    public bool IsMine { get; set; }
    public List<WorkCategoryDto> WorkCategories { get; set; } = [];
}
