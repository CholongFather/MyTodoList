namespace DevTodoList.Shared.Enums;

/// <summary>우선순위</summary>
public enum TodoPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3
}

public static class TodoPriorityExtensions
{
    public static string ToKorean(this TodoPriority p) => p switch
    {
        TodoPriority.Low => "낮음",
        TodoPriority.Normal => "보통",
        TodoPriority.High => "높음",
        TodoPriority.Urgent => "긴급",
        _ => p.ToString()
    };
}
