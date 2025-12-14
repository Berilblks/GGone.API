using GGone.API.Models.Enum;
using System.Text.Json.Serialization;

namespace GGone.API.Models.Exercises
{
    public class Exercise
    {
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("gifUrl")]
        public required string ImageUrl { get; set; }
        public ExerciseLevel ExerciseLevel { get; set; }
        public required string Description { get; set; }

        [JsonPropertyName("bodyPart")]
        public BodyPart BodyPart { get; set; }
        public required string Detail { get; set; }
        public bool IsHome { get; set; }

        [JsonPropertyName("equipment")]
        public required string EquipmentApi { get; set; }

        [JsonPropertyName("target")]
        public string? TargetApi { get; set; }

        [JsonPropertyName("instructions")]
        public List<string>? InstructionsApi { get; set; }

    }
}
