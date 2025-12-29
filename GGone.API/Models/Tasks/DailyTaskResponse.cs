namespace GGone.API.Models.Tasks
{
    public class DailyTaskResponse
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}
