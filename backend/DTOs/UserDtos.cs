using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public record RegisterRequest(
        [Required(ErrorMessage = "Username required")]
        [MaxLength(50)]
        string Name,

        [Required(ErrorMessage ="Password required")]
        [MinLength(6, ErrorMessage = "Minimum 6 symbols for password")]
        string Password);

    public record LoginRequest(
        [Required(ErrorMessage = "Username required")]
        string Name,

        [Required(ErrorMessage ="Password required")]
        string Password);
}
