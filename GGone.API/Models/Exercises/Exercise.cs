using GGone.API.Models.Enum;
using System.Text.Json.Serialization;

namespace GGone.API.Models.Exercises
{
    public class Exercise
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string ImageUrl { get; set; }
        public ExerciseLevel ExerciseLevel { get; set; }
        public required string Description { get; set; }

        public BodyPart BodyPart { get; set; }
        public required string Detail { get; set; }
        public bool IsHome { get; set; }

        public bool IsManuallyUpdated { get; set; } // Bu true ise ApiFetch dokunmayacak
    }
}
