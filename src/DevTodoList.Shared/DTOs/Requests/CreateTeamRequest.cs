using DevTodoList.Shared.Constants;

namespace DevTodoList.Shared.DTOs.Requests;

/// <summary>팀 생성/수정 요청</summary>
public class CreateTeamRequest
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = AppColors.DefaultProject;
    public bool IsMine { get; set; }
}
