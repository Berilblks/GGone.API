namespace GGone.API.Models.Tasks
{
    public class DailyTaskLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }

        public List<int> CompletedTaskIds { get; set; } = new List<int>();

    }
}
