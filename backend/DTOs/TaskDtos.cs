using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public record CreateTaskRequest(
        [Required(ErrorMessage = "Name Required")]
        [MaxLength(125, ErrorMessage = "Max name length 125 symbols")]
        string Name, 
        string? Description, 
        int? CategoryId);
    public record TaskResponse(int Id, string Name, string? Description, CategoryResponse? Category);
}
