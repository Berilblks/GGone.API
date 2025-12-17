namespace GGone.API.Models.Badges
{
    public class UserBadge
    {
        public int Id { get; set; }
        public required string BadgeId { get; set; }
        public required string Name {  get; set; }
        public required string Description { get; set; }
        public DateTime EarnedDate { get; set;  } = DateTime.UtcNow;
    }
}
