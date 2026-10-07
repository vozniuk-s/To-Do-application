using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class UserEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string HashPassword { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User";

        public ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
    }
}
