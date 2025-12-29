namespace GGone.API.Models.Friends
{
    public class FriendResponse
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Level { get; set; }
        public int Steps { get; set; }
        public bool IsFriend { get; set; }
    }
}
