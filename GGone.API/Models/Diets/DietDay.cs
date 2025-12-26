namespace GGone.API.Models.Diets
{
    public class DietDay
    {
        public int Id { get; set; }
        public string? DayName { get; set; } 
        public string? Breakfast { get; set; }
        public string? Lunch { get; set; }
        public string? Dinner { get; set; }
        public string? Snacks { get; set; }
        }
}
