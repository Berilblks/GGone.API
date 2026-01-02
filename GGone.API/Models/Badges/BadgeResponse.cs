namespace GGone.API.Models.Badges
{
    public class BadgeResponse
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Icon { get; set; }
        public required string Category { get; set; }
        public bool IsEarned { get; set; }
        public DateTime? EarnedDate { get; set; }
    }
}
