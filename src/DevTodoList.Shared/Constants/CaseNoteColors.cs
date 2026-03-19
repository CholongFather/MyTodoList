using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.Constants;

/// <summary>케이스 노트 유형별 색상</summary>
public static class CaseNoteColors
{
    public const string Comment = "#1976D2";
    public const string Guide = "#388E3C";
    public const string Workaround = "#F57C00";
    public const string RootCause = "#D32F2F";

    public static string GetColor(CaseNoteType noteType) => noteType switch
    {
        CaseNoteType.Comment => Comment,
        CaseNoteType.Guide => Guide,
        CaseNoteType.Workaround => Workaround,
        CaseNoteType.RootCause => RootCause,
        _ => Comment
    };
}
