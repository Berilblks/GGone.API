using GGone.API.Models.Auth;

namespace GGone.API.Models.Friends
{
    public class Friendship
    {
        public int Id { get; set; }
        public required int UserId { get; set; } //isteği gönderen
        public required int FriendId { get; set; } //isteği alan
        public bool IsAccepted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual User? Friend { get; set; }
    }
}
