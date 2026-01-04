namespace GGone.API.Models.AI
{
    public class ChatHistoryDto
    {
        public required string Message { get; set; }
        public bool IsUser { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
