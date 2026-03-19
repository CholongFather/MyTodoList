using System.ComponentModel.DataAnnotations;
using DevTodoList.Shared.Constants;

namespace DevTodoList.Shared.DTOs.Requests;

public class CreateTagRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = AppColors.DefaultTag;
}
