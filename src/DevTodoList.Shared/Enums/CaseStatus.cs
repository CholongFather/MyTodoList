namespace DevTodoList.Shared.Enums;

/// <summary>케이스 상태</summary>
public enum CaseStatus
{
    Open = 0,
    InProgress = 1,
    Resolved = 2,
    Closed = 3
}

public static class CaseStatusExtensions
{
    public static string ToKorean(this CaseStatus s) => s switch
    {
        CaseStatus.Open => "열림",
        CaseStatus.InProgress => "진행중",
        CaseStatus.Resolved => "해결",
        CaseStatus.Closed => "종료",
        _ => s.ToString()
    };
}
