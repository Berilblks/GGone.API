using GGone.API.Business.Abstracts;
using GGone.API.Data;
using GGone.API.Models;
using GGone.API.Models.Enum;
using GGone.API.Models.Exercises; 
using Microsoft.Extensions.Configuration; 
using System.Text.Json;

namespace GGone.API.Business.Services.Exercises
{
    public class RapidExerciseDataFetcher : IExerciseDataFetcher
    {
        private readonly IConfiguration _configuration;
        private readonly GGoneDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly string _apiHost;

        public RapidExerciseDataFetcher(IConfiguration configuration, GGoneDbContext context)
        {
            _configuration = configuration;
            _context = context;
            _httpClient = new HttpClient();

            // Yapılandırmadan API anahtarlarını okuma
            _apiKey = _configuration["ExternalApis:RapidApiExerciseKey"]
                      ?? throw new InvalidOperationException("API Key not found.");
            _apiUrl = _configuration["ExternalApis:RapidApiUrl"]
                      ?? throw new InvalidOperationException("API URL not found.");
            _apiHost = _configuration["ExternalApis:RapidApiHost"]
                       ?? throw new InvalidOperationException("API Host not found.");
        }

        public async Task<BaseResponse<string>> FetchAndSaveAllExercises()
        {
            try
            {
                // HTTP isteği oluşturma ve API Key'i başlığa (Header) ekleme
                var request = new HttpRequestMessage(HttpMethod.Get, _apiUrl);
                request.Headers.Add("X-RapidAPI-Key", _apiKey);
                request.Headers.Add("X-RapidAPI-Host", _apiHost);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return new BaseResponse<string>(false, $"API İsteği Başarısız: {response.ReasonPhrase}");
                }

                var jsonBody = await response.Content.ReadAsStringAsync();

                // JSON'ı doğrudan Exercise model listesine dönüştürme
                var exercises = JsonSerializer.Deserialize<List<Exercise>>(
                    jsonBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (exercises == null || !exercises.Any())
                {
                    return new BaseResponse<string>(true, "API'den egzersiz verisi gelmedi.");
                }

                // 4. Veri tabanına kaydetme (Mapping işlemi burada gerçekleşecek)
                int savedCount = await SaveToDatabase(exercises);

                return new BaseResponse<string>(true, $"{savedCount} adet egzersiz başarıyla veritabanına kaydedildi.");
            }
            catch (Exception ex)
            {
                return new BaseResponse<string>(false, $"Veri çekme veya kaydetme hatası: {ex.Message}");
            }
        }

        // --- Yardımcı Metotlar (Mapping ve Kayıt) ---

        private async Task<int> SaveToDatabase(List<Exercise> exercises)
        {
            var finalizedExercises = new List<Exercise>();

            foreach (var exercise in exercises)
            {
                // 1. Mapping: Geçici API alanlarını (EquipmentApi, TargetApi) kullanarak final alanları doldurma
                exercise.BodyPart = MapBodyPart(exercise.BodyPart.ToString());
                exercise.ExerciseLevel = MapLevel(exercise.EquipmentApi);

                // 2. Açıklama ve Detay alanlarını doldurma
                exercise.Description = $"Hedef: {exercise.TargetApi}. Ekipman: {exercise.EquipmentApi}";
                exercise.Detail = string.Join("\n", exercise.InstructionsApi ?? new List<string>());

                // 3. IsHome alanı
                exercise.IsHome = exercise.EquipmentApi?.ToLower() == "body weight";

                finalizedExercises.Add(exercise);
            }

            await _context.Exercises.AddRangeAsync(finalizedExercises);
            await _context.SaveChangesAsync();

            return finalizedExercises.Count;
        }

        private BodyPart MapBodyPart(string apiBodyPart)
        {
            return apiBodyPart?.ToLower() switch
            {
                "back" => BodyPart.Back,
                "chest" => BodyPart.Chest,
                "core" => BodyPart.Abs,
                "lower legs" or "upper legs" => BodyPart.Legs,
                "shoulders" => BodyPart.Shoulders,
                _ => BodyPart.Abs,
            };
        }

        private ExerciseLevel MapLevel(string apiEquipment)
        {
            return apiEquipment?.ToLower() switch
            {
                "body weight" => ExerciseLevel.Beginner,
                "barbell" or "dumbbell" or "cable" => ExerciseLevel.Advanced,
                _ => ExerciseLevel.Intermediate,
            };
        }
    }
}
