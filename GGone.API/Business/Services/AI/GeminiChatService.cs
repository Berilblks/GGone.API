using GGone.API.Business.Abstracts;
using GGone.API.Models;
using GGone.API.Models.AI;
using GGone.API.Prompting;
using System.Text.Json;

namespace GGone.API.Business.Services.AI
{
    public class GeminiChatService : IAIChatService
    {
        private readonly HttpClient _httpClient;
        private readonly IBmiService _bmiService;
        private readonly ICurrentUserService _currentUserService;
        private readonly string _apiKey;

        public GeminiChatService(IConfiguration configuration, HttpClient httpClient, IBmiService bmiService, ICurrentUserService currentUserService)
        {
            _apiKey = configuration["GeminiSettings:ApiKey"]
                ?? throw new Exception("Gemini API Key bulunamadı!");

            _httpClient = httpClient; // 🔥 ÖNCE ATA

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            _bmiService = bmiService;
            _currentUserService = currentUserService;
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

                // Gemini payload (ROLE ŞART!)
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
                    // Burayı izle (Watch): Google hatanın nedenini (geçersiz anahtar, yanlış format vb.) burada açıklar.
                    Console.WriteLine($"GOOGLE HATA DETAYI: {errorJson}");

                    return new BaseResponse<AIChatResponse> { Success = false, Message = "Google hatası: " + response.StatusCode };
                }

                // 5️⃣ Response parse
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
                    // Gemini bazen safety veya boş cevap döner
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
            
    }
}
