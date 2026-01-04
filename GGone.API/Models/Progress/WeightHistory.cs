namespace GGone.API.Models.Progress
{
    public class WeightHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double Weight { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
