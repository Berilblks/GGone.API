using System.Text.Json.Serialization;

namespace GGone.API.Models.Exercises
{
    // Wger API sayfalı yanıt döndürür (Wrapper)
    public class WgerResponseDto
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("next")]
        public string? Next { get; set; }

        [JsonPropertyName("results")]
        public List<ExerciseApiDto>? Results { get; set; }
    }

    // Her bir egzersizin detayı
    public class ExerciseApiDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        // Artık name ve description direct properties değil
        // Translations içinden alacağız

        [JsonPropertyName("category")]
        public WgerCategoryDto? Category { get; set; }

        [JsonPropertyName("equipment")]
        public List<WgerEquipmentDto>? Equipment { get; set; }

        [JsonPropertyName("images")]
        public List<WgerImageDto>? Images { get; set; }
        
        [JsonPropertyName("translations")]
        public List<WgerTranslationDto>? Translations { get; set; }
    }

    public class WgerTranslationDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("language")]
        public int LanguageId { get; set; } // 2 = English
        
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        
        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public class WgerCategoryDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class WgerEquipmentDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class WgerImageDto
    {
        [JsonPropertyName("image")]
        public string? Image { get; set; }
    }
}
