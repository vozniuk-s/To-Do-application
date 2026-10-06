using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public record CreateCategoryRequest(
        [Required(ErrorMessage = "Name Required")]
        [MaxLength(40, ErrorMessage = "Max name length 40 symbols")]
        string Name);
    public record UpdateCategoryRequest(
        [Required(ErrorMessage = "Name Required")]
        [MaxLength(40, ErrorMessage = "Max name length 40 symbols")]
        string Name);
    public record CategoryResponse (int Id, string Name);
}
