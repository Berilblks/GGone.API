using GGone.API.Business.Abstracts;
using GGone.API.Data;
using GGone.API.Models;
using GGone.API.Models.Enum;
using GGone.API.Models.Exercises; 
using Microsoft.Extensions.Configuration; 
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GGone.API.Business.Services.Exercises
{
    public class RapidExerciseDataFetcher : IExerciseDataFetcher
    {
        private readonly IConfiguration _configuration;
        private readonly GGoneDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public RapidExerciseDataFetcher(IConfiguration configuration, GGoneDbContext context)
        {
            _configuration = configuration;
            _context = context;
            _httpClient = new HttpClient();

            // appsettings'den Wger URL alıyoruz
            _apiUrl = _configuration["ExternalApis:RapidApiUrl"] 
                      ?? "https://wger.de/api/v2/exerciseinfo/";
        }

        public async Task<BaseResponse<string>> FetchAndSaveAllExercises()
        {
            try
            {
                // Wger public olduğu için Key/Host header'a gerek yok.
                // Sayfalama (Pagination) mantığı: next url null olana kadar döneriz.
                // Güvenlik için şimdilik ilk 10 sayfayı (200 egzersiz) çekelim, sonra artırabiliriz.
                
                // Dil: İngilizce (2). Limit: 50.
                string? nextUrl = $"{_apiUrl}?language=2&limit=50"; 
                int totalSaved = 0;
                int totalFetched = 0; 
                int safetyCounter = 0; 
                
                
                while (!string.IsNullOrEmpty(nextUrl) && safetyCounter < 20) // 1000 egzersiz limiti
                {
                    var response = await _httpClient.GetAsync(nextUrl);

                    if (!response.IsSuccessStatusCode)
                        return BaseResponse<string>.Fail($"Wger API Hatası: {response.ReasonPhrase} (Status: {response.StatusCode})");

                    var jsonBody = await response.Content.ReadAsStringAsync();

                    var wgerResponse = JsonSerializer.Deserialize<WgerResponseDto>(
                         jsonBody,
                         new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (wgerResponse?.Results == null || !wgerResponse.Results.Any())
                        break;

                    totalFetched += wgerResponse.Results.Count;
                    totalSaved += await SaveToDatabase(wgerResponse.Results);
                    
                    nextUrl = wgerResponse.Next; 
                    safetyCounter++;
                }

                return BaseResponse<string>.Ok($"Tüm egzersiz verileri güncellendi. {totalFetched} egzersiz tarandı, {totalSaved} kayıtta bilgi güncellemesi (Vücut bölgesi/Ekipman/Resim) yapıldı.");
            }
            catch (Exception ex)
            {
                 // Hata durumunda log
                 var sb = new System.Text.StringBuilder();
                 sb.AppendLine($"Hata: {ex.Message}");
                 if(ex.InnerException != null) sb.AppendLine($"Inner: {ex.InnerException.Message}");
                 return BaseResponse<string>.Fail(sb.ToString());
            }
        }

        private async Task<int> SaveToDatabase(List<ExerciseApiDto> dtos)
        {
            var newEntities = new List<Exercise>();
            int processedCount = 0;

            foreach (var dto in dtos)
            {
                var translation = dto.Translations?.FirstOrDefault(t => t.LanguageId == 2) 
                                  ?? dto.Translations?.FirstOrDefault();

                string finalName = translation?.Name ?? "";
                string finalDesc = translation?.Description ?? "";
                string finalImage = dto.Images?.FirstOrDefault()?.Image ?? "";

                if (string.IsNullOrWhiteSpace(finalName) || finalName.Length < 3) continue;

                // İsmi de gönderiyoruz ki manuel kontrol yapabilelim
                var newBodyPart = MapWgerCategory(dto.Category?.Name, finalName);
                var newLevel = MapWgerEquipment(dto.Equipment);
                var newIsHome = IsHomeFriendly(dto.Equipment, finalName);

                // Mevcut kaydı kontrol et
                var existingEntity = _context.Exercises.FirstOrDefault(e => e.Name == finalName);
                if (existingEntity != null)
                {
                    // KORUMA KALKANI: Eğer elle güncellendiyse DOKUNMA! 🛡️
                    if (existingEntity.IsManuallyUpdated) continue;

                    bool updated = false;

                    if (!string.IsNullOrEmpty(finalImage) && existingEntity.ImageUrl != finalImage)
                    {
                        existingEntity.ImageUrl = finalImage;
                        updated = true;
                    }
                    
                    if ((string.IsNullOrEmpty(existingEntity.Description) || existingEntity.Description.Length < 10) && 
                        !string.IsNullOrEmpty(finalDesc) && finalDesc.Length > 10)
                    {
                        existingEntity.Description = CleanHtml(finalDesc);
                        existingEntity.Detail = CleanHtml(finalDesc);
                        updated = true;
                    }

                    if (existingEntity.BodyPart != newBodyPart)
                    {
                        existingEntity.BodyPart = newBodyPart;
                        updated = true;
                    }

                    if (existingEntity.ExerciseLevel != newLevel)
                    {
                        existingEntity.ExerciseLevel = newLevel;
                        updated = true;
                    }

                    if (existingEntity.IsHome != newIsHome)
                    {
                        existingEntity.IsHome = newIsHome;
                        updated = true;
                    }

                    if (updated) processedCount++;
                    continue; 
                }

                var entity = new Exercise
                {
                    Name = finalName,
                    ImageUrl = finalImage, 
                    BodyPart = newBodyPart,
                    ExerciseLevel = newLevel,
                    Description = CleanHtml(finalDesc),
                    Detail = CleanHtml(finalDesc),
                    IsHome = newIsHome
                };

                newEntities.Add(entity);
                processedCount++;
            }

            if (newEntities.Any())
            {
                await _context.Exercises.AddRangeAsync(newEntities);
            }

            await _context.SaveChangesAsync();
            return processedCount;
        }

        // --- Yardımcı Metotlar ---

        private BodyPart MapWgerCategory(string? categoryName, string exerciseName)
        {
            // 1. MANUEL MÜDAHALE ALANI (İstediğiniz egzersizi elle buraya yazabilirsiniz)
            // Örnek: Adında 'Squat' geçen her şeyi Bacak yap
            string nameLower = exerciseName.ToLower();

            if (nameLower.Contains("squat") || nameLower.Contains("lunges")) return BodyPart.Legs;
            if (nameLower.Contains("bench press") || nameLower.Contains("push up")) return BodyPart.Chest;
            if (nameLower.Contains("curl")) return BodyPart.Arms;
            if (nameLower.Contains("plank") || nameLower.Contains("crunch")) return BodyPart.Abs;
            if (nameLower.Contains("pull up") || nameLower.Contains("row")) return BodyPart.Back;
            if (nameLower.Contains("press") && nameLower.Contains("shoulder")) return BodyPart.Shoulders;

            // 2. Otomatik Wger Mapping
            return categoryName?.ToLower() switch
            {
                "arms" => BodyPart.Arms, 
                "shoulders" => BodyPart.Shoulders,
                "chest" => BodyPart.Chest,
                "back" => BodyPart.Back,
                "legs" or "calves" or "cardio" => BodyPart.Legs, 
                "abs" => BodyPart.Abs,
                _ => BodyPart.Abs 
            };
        }

        private ExerciseLevel MapWgerEquipment(List<WgerEquipmentDto>? equipmentList)
        {
            if (equipmentList == null || !equipmentList.Any()) 
                return ExerciseLevel.Beginner; 

            if (equipmentList.Any(e => e.Name.ToLower().Contains("barbell") || 
                                       e.Name.ToLower().Contains("machine") ||
                                       e.Name.ToLower().Contains("cable") ||
                                       e.Name.ToLower().Contains("sz-bar")))
                return ExerciseLevel.Advanced;

            if (equipmentList.Any(e => e.Name.ToLower().Contains("dumbbell") || 
                                       e.Name.ToLower().Contains("kettlebell") ||
                                       e.Name.ToLower().Contains("bench") ||
                                       e.Name.ToLower().Contains("pull-up")))
                return ExerciseLevel.Intermediate;

            return ExerciseLevel.Beginner;
        }

        private bool IsHomeFriendly(List<WgerEquipmentDto>? equipmentList, string exerciseName)
        {
            // 1. MANUEL MÜDAHALE (Evde yapılabilir mi?)
            string nameLower = exerciseName.ToLower();
            
            // Mekik, sınav vb. her zaman evde yapılır
            if (nameLower.Contains("push up") || nameLower.Contains("crunch") || nameLower.Contains("plank")) return true;
            
            // 2. Otomatik Kontrol
            if (equipmentList == null || !equipmentList.Any()) return true;

            return equipmentList.All(e => 
            {
                var name = e.Name.ToLower();
                return name.Contains("mat") || 
                       name.Contains("dumbbell") ||
                       name.Contains("kettlebell") ||
                       name.Contains("body weight") ||
                       name.Contains("none") ||
                       name.Contains("swiss ball") ||
                       name.Contains("resistance band") ||
                       name.Contains("pull-up"); 
            });
        }

        private string CleanHtml(string html)
        {
            if (string.IsNullOrEmpty(html)) return "Açıklama yok.";
            return Regex.Replace(html, "<.*?>", string.Empty).Trim();
        }
    }
}
