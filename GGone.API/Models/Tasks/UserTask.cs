using GGone.API.Models.Auth;
using System.ComponentModel.DataAnnotations.Schema;

namespace GGone.API.Models.Tasks
{
    public class UserTask
    {
        public int Id { get; set; }
        public required int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        public required int TaskId { get; set; }

        [ForeignKey(nameof(TaskId))]
        public Task? Task { get; set; }
        public required DateTime AssignedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
