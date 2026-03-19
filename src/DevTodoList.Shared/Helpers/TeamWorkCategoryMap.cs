using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.Helpers;

/// <summary>담당자 유형 라벨 헬퍼 (팀/작업분류는 DB에서 관리)</summary>
public static class AssigneeTypeHelper
{
    /// <summary>담당자 유형 한글명</summary>
    public static string GetLabel(AssigneeType type) => type switch
    {
        AssigneeType.Mine => "내 작업",
        AssigneeType.Others => "타인 작업",
        _ => type.ToString()
    };
}
