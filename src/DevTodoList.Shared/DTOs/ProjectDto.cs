using DevTodoList.Shared.Constants;

namespace DevTodoList.Shared.DTOs;

public class ProjectDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = AppColors.DefaultProject;
    public int SortOrder { get; set; }
    public bool IsArchived { get; set; }
    public int TodoCount { get; set; }
    public int ActiveCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ProjectMetaDto> Metas { get; set; } = [];
}
