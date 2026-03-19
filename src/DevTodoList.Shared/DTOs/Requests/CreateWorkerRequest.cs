namespace DevTodoList.Shared.DTOs.Requests;

/// <summary>작업자 생성/수정 요청</summary>
public class CreateWorkerRequest
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#9E9E9E";
    public bool IsMe { get; set; }
}
