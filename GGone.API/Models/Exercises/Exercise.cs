using GGone.API.Models.Enum;

namespace GGone.API.Models.Exercises
{
    public class Exercise
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ExerciseLevel ExerciseLevel { get; set; }
        public required string Photo { get; set; }
        public required string Description { get; set; }
        public BodyPart BodyPart { get; set; }
        public required string Detail { get; set; }

    }
}
