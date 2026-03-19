using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs.Requests;

/// <summary>일정 텍스트 파싱 요청</summary>
public class ScheduleParseRequest
{
    /// <summary>파싱할 일정 텍스트</summary>
    public string Text { get; set; } = string.Empty;
    /// <summary>생성할 프로젝트 ID</summary>
    public long ProjectId { get; set; }

    /// <summary>기본 담당 팀 ID</summary>
    public long? DefaultTeamId { get; set; }
    /// <summary>기본 작업 분류 ID</summary>
    public long? DefaultWorkCategoryId { get; set; }
    /// <summary>기본 담당자 유형</summary>
    public AssigneeType DefaultAssigneeType { get; set; } = AssigneeType.Mine;
    /// <summary>기본 담당 유형 ID (AssigneeTypeEntity FK)</summary>
    public long? DefaultAssigneeTypeId { get; set; }
    /// <summary>외부 일정 여부</summary>
    public bool DefaultIsExternal { get; set; }
}
