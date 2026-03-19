namespace DevTodoList.Shared.Enums;

/// <summary>출퇴근 구분</summary>
public enum CommuteType
{
    Morning = 0,  // 출근
    Evening = 1   // 퇴근
}

public static class CommuteTypeExtensions
{
    public static string ToKorean(this CommuteType type) =>
        type == CommuteType.Morning ? "출근" : "퇴근";
}
