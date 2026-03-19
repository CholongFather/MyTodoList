namespace DevTodoList.Shared.Enums;

/// <summary>담당자 유형</summary>
public enum AssigneeType
{
    /// <summary>내 작업 - 수동 완료만 가능</summary>
    Mine = 0,
    /// <summary>타인 작업 - 기한 경과 시 연장/완료 프롬프트</summary>
    Others = 1
}

public static class AssigneeTypeExtensions
{
    public static string ToKorean(this AssigneeType a) => a switch
    {
        AssigneeType.Mine => "내 작업",
        AssigneeType.Others => "타인 작업",
        _ => a.ToString()
    };
}
