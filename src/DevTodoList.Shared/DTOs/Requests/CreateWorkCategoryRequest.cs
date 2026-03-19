namespace DevTodoList.Shared.DTOs.Requests;

/// <summary>작업 분류 생성/수정 요청</summary>
public class CreateWorkCategoryRequest
{
    public string Name { get; set; } = string.Empty;
}
