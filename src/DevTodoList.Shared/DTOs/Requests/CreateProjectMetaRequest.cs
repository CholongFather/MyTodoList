using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs.Requests;

/// <summary>프로젝트 메타 생성/수정 요청</summary>
public class CreateProjectMetaRequest
{
    public ProjectMetaType MetaType { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
