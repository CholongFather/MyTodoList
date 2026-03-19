namespace DevTodoList.Shared.Enums;

/// <summary>TODO 상태</summary>
public enum TodoStatus
{
    Todo = 0,
    InProgress = 1,
    Done = 2,
    Archived = 3
}

public static class TodoStatusExtensions
{
    public static string ToKorean(this TodoStatus s) => s switch
    {
        TodoStatus.Todo => "할일",
        TodoStatus.InProgress => "진행중",
        TodoStatus.Done => "완료",
        TodoStatus.Archived => "보관",
        _ => s.ToString()
    };
}
