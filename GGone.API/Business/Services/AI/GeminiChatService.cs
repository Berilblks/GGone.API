using AutoMapper;
using GGone.API.Business.Abstracts;
using GGone.API.Data;
using GGone.API.Models;
using GGone.API.Models.AI;
using GGone.API.Models.Diets;
using GGone.API.Prompting;
using Microsoft.EntityFrameworkCore;
using Polly;
using System.Text.Json;

namespace GGone.API.Business.Services.AI
{
    public class GeminiChatService : IAIChatService
    {
        private readonly HttpClient _httpClient;
        private readonly IBmiService _bmiService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly GGoneDbContext _context;
        private readonly string _apiKey;

        public GeminiChatService(IConfiguration configuration, HttpClient httpClient, IBmiService bmiService, ICurrentUserService currentUserService, IMapper mapper, GGoneDbContext context)
        {
            _apiKey = configuration["GeminiSettings:ApiKey"]
                ?? throw new Exception("Gemini API Key bulunamadı!");
            _httpClient = httpClient;
            _bmiService = bmiService;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _context = context;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        }

        public async Task<BaseResponse<AIChatResponse>> GetAiReply(AIChatRequest request)
        {
            try
            {
                // Kullanıcı & BMI bilgisi
                var userId = _currentUserService.UserId;
                var lastBmi = await _bmiService.GetLatestBmiByUserId(userId);

                double bmiValue = lastBmi?.BmiResult ?? 0;

                string userMessage = string.IsNullOrWhiteSpace(request.Message)
                    ? "Kilo vermek istiyorum"
                    : request.Message;

                // Prompt oluştur
                string fullPrompt =
                    SystemPrompts.CoachRole +
                    "\n\n" +
                    UserContextBuilder.Build(
                        userMessage,
                        bmiValue,
                        "Kilo Vermek"
                    );

                // Gemini payload 
                var payload = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = fullPrompt }
                            }
                        }
                    }
                };

                // Gemini API çağrısı
                var response = await _httpClient.PostAsJsonAsync(
                    $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}",
                    payload
                );




                if (!response.IsSuccessStatusCode)
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"GOOGLE HATA DETAYI: {errorJson}");

                    return new BaseResponse<AIChatResponse> { Success = false, Message = "Google hatası: " + response.StatusCode };
                }

                // Response parse
                var json = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(json);

                string aiText;

                try
                {
                    aiText = doc
                        .RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString()
                        ?? "Şu an net bir yanıt üretemedim.";
                }
                catch
                {
                    Console.WriteLine("Gemini boş veya hatalı response döndü:");
                    Console.WriteLine(json);

                    aiText = "Şu an yanıt üretilemedi. Lütfen tekrar deneyin.";
                }

                return new BaseResponse<AIChatResponse>
                {
                    Success = true,
                    Data = new AIChatResponse
                    {
                        Reply = aiText
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GeminiChatService Exception: {ex.Message}");

                return new BaseResponse<AIChatResponse>
                {
                    Success = false,
                    Message = "Beklenmeyen bir hata oluştu."
                };
            }
        }

        public async Task<BaseResponse<WeeklyDietPlan>> GenerateWeeklyDietPlan()
        {
            try
            {
                var userId = _currentUserService.UserId;
                var lastBmi = await _bmiService.GetLatestBmiByUserId(userId);

                string dietPrompt = $"BMI: {lastBmi?.BmiResult ?? 25}. 7 günlük diyet listesini SADECE JSON olarak hazırla. " +
                            "JSON yapısı 'Days' listesinden oluşmalı ve her gün 'DayName', 'Breakfast', 'Lunch', 'Dinner', 'Snacks' içermeli.";

                // Gemini'ye özel Payload (JSON modu aktif)
                var payload = new
                {
                    contents = new[] { new { parts = new[] { new { text = dietPrompt } } } },
                    generationConfig = new { response_mime_type = "application/json" }
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}",
                    payload
                );

                if (!response.IsSuccessStatusCode)
                    return new BaseResponse<WeeklyDietPlan> { Success = false, Message = "Gemini API Hatası" };

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                string aiRawJson = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? "{}";

                // 1. JSON -> WeeklyDietPlan (Ham Veri)
                var rawDiet = JsonSerializer.Deserialize<WeeklyDietPlan>(aiRawJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // 2. Mapleyerek temiz entity oluştur (Profile üzerinden)
                var dietEntity = _mapper.Map<WeeklyDietPlan>(rawDiet);

                dietEntity.UserId = userId;
                dietEntity.CreatedAt = DateTime.UtcNow;

                // 3. Veritabanına kaydet
                _context.WeeklyDietPlans.Add(dietEntity);
                await _context.SaveChangesAsync();

                return new BaseResponse<WeeklyDietPlan> { Success = true, Data = dietEntity };
            }
            catch (Exception ex)
            {
                return new BaseResponse<WeeklyDietPlan> { Success = false, Message = "Diyet oluşturulamadı: " + ex.Message };
            }
        }

        public async Task<BaseResponse<WeeklyDietPlan>> GetUserDietPlan()
        {
            try
            {
                var userId = _currentUserService.UserId; // Giriş yapan kullanıcının ID'si

                var dietPlan = await _context.WeeklyDietPlans
                    .Include(x => x.Days) // Gün detaylarını (DietDay) dahil et
                    .OrderByDescending(x => x.CreatedAt) // En son oluşturulanı getir
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (dietPlan == null)
                    return new BaseResponse<WeeklyDietPlan> { Success = false, Message = "Henüz bir diyet planı oluşturulmamış." };

                    return new BaseResponse<WeeklyDietPlan> { Success = true, Data = dietPlan };
            }
            catch (Exception ex)
            {
                return new BaseResponse<WeeklyDietPlan> { Success = false, Message = "Diyet planı getirilirken hata oluştu." };
            }
        }
    }
}
