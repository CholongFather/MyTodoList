using System.ComponentModel.DataAnnotations;

namespace DevTodoList.Shared.DTOs.Requests;

public class CreateCheckItemRequest
{
    [Required]
    [StringLength(500)]
    public string Title { get; set; } = string.Empty;
}
