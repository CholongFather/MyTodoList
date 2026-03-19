namespace DevTodoList.Shared.Enums;

/// <summary>케이스 카테고리</summary>
public enum CaseCategory
{
    Bug = 0,
    Enhancement = 1,
    Question = 2,
    Task = 3
}

public static class CaseCategoryExtensions
{
    public static string ToKorean(this CaseCategory c) => c switch
    {
        CaseCategory.Bug => "버그",
        CaseCategory.Enhancement => "개선",
        CaseCategory.Question => "질문",
        CaseCategory.Task => "작업",
        _ => c.ToString()
    };
}
