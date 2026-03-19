namespace DevTodoList.Shared.DTOs.Requests;

/// <summary>할일 작업자 목록 갱신 요청</summary>
public class UpdateWorkersRequest
{
    public List<long> WorkerIds { get; set; } = [];
}
