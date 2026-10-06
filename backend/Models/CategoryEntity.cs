using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class CategoryEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; } = string.Empty;

        public ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
    }
}
