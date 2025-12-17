using System.ComponentModel.DataAnnotations;

namespace GGone.API.Models.Tasks
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string TaskId { get; set; } 

        [Required]
        [MaxLength(100)]
        public required string Title { get; set; }

        [Required]
        [MaxLength(500)]
        public required string Description { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Category { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
