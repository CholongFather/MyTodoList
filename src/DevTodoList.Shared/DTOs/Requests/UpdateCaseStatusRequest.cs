using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs.Requests;

/// <summary>케이스 상태 변경 요청</summary>
public class UpdateCaseStatusRequest
{
    public CaseStatus Status { get; set; }
}
