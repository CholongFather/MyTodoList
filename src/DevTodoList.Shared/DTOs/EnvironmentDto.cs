using DevTodoList.Shared.Constants;

namespace DevTodoList.Shared.DTOs;

/// <summary>배포 환경 DTO</summary>
public class EnvironmentDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = AppColors.DefaultProject;
    public int SortOrder { get; set; }
}
