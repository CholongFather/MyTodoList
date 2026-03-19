using System.ComponentModel.DataAnnotations;
using DevTodoList.Shared.Constants;

namespace DevTodoList.Shared.DTOs.Requests;

/// <summary>환경 생성/수정 요청</summary>
public class CreateEnvironmentRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = AppColors.DefaultProject;
}
