namespace GGone.API.Models.Diets
{
    public class WeeklyDietPlan
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<DietDay> Days { get; set; } = new();
    }
}
