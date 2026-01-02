namespace GGone.API.Models.Badges
{
    public class BadgeDefinition
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Icon { get; set; } // Emojis here
        public required string Category { get; set; }
    }
}
