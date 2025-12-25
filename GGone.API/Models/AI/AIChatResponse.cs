namespace GGone.API.Models.AI
{
    public class AIChatResponse
    {
        public string Reply { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}
