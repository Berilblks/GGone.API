using GGone.API.Models.Enum;

namespace GGone.API.Models.Exercises
{
    public class ExerciseFilterRequest
    {
        public bool? IsHome { get; set; }
        public BodyPart? BodyPart { get; set; }
        public ExerciseLevel? ExerciseLevel { get; set; }
        public string? Name { get; set; }

    }
}
