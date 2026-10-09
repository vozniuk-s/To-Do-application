using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class TaskEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(125)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public int? CategoryId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("CategoryId")]
        public CategoryEntity? Category { get; set; }
    }
}
