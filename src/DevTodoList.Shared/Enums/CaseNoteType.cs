namespace DevTodoList.Shared.Enums;

/// <summary>케이스 노트 유형</summary>
public enum CaseNoteType
{
    Comment = 0,
    Guide = 1,
    Workaround = 2,
    RootCause = 3
}

public static class CaseNoteTypeExtensions
{
    public static string ToKorean(this CaseNoteType t) => t switch
    {
        CaseNoteType.Comment => "댓글",
        CaseNoteType.Guide => "가이드",
        CaseNoteType.Workaround => "우회방법",
        CaseNoteType.RootCause => "근본원인",
        _ => t.ToString()
    };
}
