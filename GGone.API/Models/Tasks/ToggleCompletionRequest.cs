using System.ComponentModel.DataAnnotations;

namespace GGone.API.Models.Tasks
{
    public class ToggleCompletionRequest
    {
        [Required]
        public required string TaskId { get; set; }

        [Required]
        public bool IsCompleted { get; set; }
    }
}
