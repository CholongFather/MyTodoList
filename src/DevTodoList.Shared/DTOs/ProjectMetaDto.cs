using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs;

/// <summary>프로젝트 메타 정보 DTO</summary>
public class ProjectMetaDto
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public ProjectMetaType MetaType { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
