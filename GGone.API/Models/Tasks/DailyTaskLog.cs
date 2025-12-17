namespace GGone.API.Models.Tasks
{
    public class DailyTaskLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public List<string> CompletedTaskIds { get; set; } = new List<string>();

    }
}
