using GGone.API.Models.Enum;

namespace GGone.API.Models.Exercises
{
    public class ExerciseResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? ImageUrl { get; set; }
        public ExerciseLevel ExerciseLevel { get; set; }
        public BodyPart BodyPart { get; set; }
        public string? Description { get; set; }
        public string? Detail { get; set; }
        public bool IsHome { get; set; }
    }
}
