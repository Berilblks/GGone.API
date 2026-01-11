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
            _apiKey = configuration["GeminiApiKey"]
                ?? throw new Exception("Gemini API Key not found!");
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
                var userId = _currentUserService.UserId;
                var lastBmi = await _bmiService.GetLatestBmiByUserId();
                double bmiValue = lastBmi?.BmiResult ?? 0;

                string userMessage = string.IsNullOrWhiteSpace(request.Message)
                    ? "I want to lose weight"
                    : request.Message;

                // 1. Kullanıcı mesajını kaydet
                var userHistory = new ChatHistory
                {
                    UserId = userId,
                    Role = "user",
                    Message = userMessage,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ChatHistories.Add(userHistory);
                await _context.SaveChangesAsync();

                // 2. Geçmiş mesajları getir (Son 100 mesaj - Kullanıcı isteği ile artırıldı)
                var history = await _context.ChatHistories
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(100)
                    .OrderBy(x => x.CreatedAt)
                    .ToListAsync();

                // 3. Payload oluştur (Smart Windowing)
                // Hedefimiz: Request Too Long hatası almamak için token/karakter sınırını aşmamak.
                // En son mesajı kesinlikle ekle, geriye doğru kapasite yettikçe ekle.
                
                var contents = new List<object>();

                var user = await _context.Users.FindAsync(userId);
                int age = 0;
                if(user != null)
                {
                   var today = DateOnly.FromDateTime(DateTime.UtcNow);
                   age = today.Year - user.BirthDate.Year;
                   if (user.BirthDate > today.AddYears(-age)) age--;
                }

                // Calculate Last Message Context Size first
                var currentDiet = await _context.WeeklyDietPlans
                   .Include(d => d.Days)
                   .Where(x => x.UserId == userId)
                   .OrderByDescending(x => x.CreatedAt)
                   .FirstOrDefaultAsync();

                string? dietContext = null;
                if (currentDiet != null)
                {
                   dietContext = JsonSerializer.Serialize(currentDiet.Days, new JsonSerializerOptions { WriteIndented = false });
                }

                // Son mesajı oluştur (Context ile birlikte)
                string lastMessageText = UserContextBuilder.Build(userHistory.Message, bmiValue, 
                           user?.Weight ?? 0, 
                           user?.Height ?? 0, 
                           age, 
                           user?.Gender ?? "Unknown",
                           dietContext);

                int currentTotalChars = lastMessageText.Length;
                const int MAX_TOTAL_CHARS = 25000; // Güvenli sınır (Gemini Flash için token sınırı yüksek olsa da request body sınırı olabilir)

                // Geriye doğru taranacak mesajlar (en sondan başa doğru bakıp sığanları alacağız)
                // History listesi zaten eskiden yeniye sıralı olduğu için reverse yapıp sondan geriye gidelim.
                var reversedHistory = history.OrderByDescending(x => x.CreatedAt).ToList();
                var messagesToSend = new List<ChatHistory>();

                // Şu anki kullanıcı mesajı zaten eklenecek, onu history'den değil manuel ekleyeceğiz.
                // History'de şu anki mesaj yoksa (yukarıda veritabanına ekledikten sonra çektik mi? Evet 62. satırda çekiyoruz ama EF gecikmesi olabilir mi? 
                // Kod akışına göre 58'de ekliyoruz, 62'de çekiyoruz. Yani history içinde userHistory de VAR.
                // userHistory'yi zaten lastMessageText olarak özel hazırladık. O yüzden history listesinden onu exclude edip diğerlerine bakacağız.

                foreach (var item in reversedHistory)
                {
                    if (item.Id == userHistory.Id) continue; // Son mesajı atla, onu en son özel ekleyeceğiz.

                    if (currentTotalChars + item.Message.Length < MAX_TOTAL_CHARS)
                    {
                        messagesToSend.Add(item);
                        currentTotalChars += item.Message.Length;
                    }
                    else
                    {
                        // Sınır aşıldı, daha eski mesajları alma
                        break; 
                    }
                }

                // Mesajları tekrar kronolojik sıraya sok (Eskiden Yeniye)
                messagesToSend.Reverse();

                // Şimdi contents listesini doldur
                foreach (var item in messagesToSend)
                {
                     contents.Add(new
                     {
                         role = item.Role,
                         parts = new[] { new { text = item.Message } }
                     });
                }

                // En son "şimdiki" mesajı ekle (Contextli haliyle)
                contents.Add(new
                {
                    role = userHistory.Role,
                    parts = new[] { new { text = lastMessageText } }
                });

                // ** YENİ EKLENTİ: Egzersiz Listesini System Prompt'a Ekle
                // Eğer kullanıcı "workout" veya "antrenman" kelimesi geçiriyorsa contexti yükle.
                // Her seferinde yüklemek token maliyetini artırır ama en doğrusu budur.
                string systemInstructionText = SystemPrompts.CoachRole;

                if (userMessage.ToLower().Contains("workout") || 
                    userMessage.ToLower().Contains("training") || 
                    userMessage.ToLower().Contains("program") ||
                    userMessage.ToLower().Contains("antrenman"))
                {
                    string exerciseListContext = await GetExerciseContext();
                    systemInstructionText += $"\n\nAVAILABLE EXERCISES (DATABASE):\n{exerciseListContext}\n\n" +
                                             "WORKOUT GENERATION RULES:\n" +
                                             "1. If the user asks for a workout plan, FIRST ask clarifying questions (Goal, Days/Week, Equipment/Location) if not already provided.\n" +
                                             "2. Generate a plan using ONLY the exercises listed above. Do not invent new exercises.\n" +
                                             "3. Use the EXACT 'Name' from the list for the 'Name' field in JSON.\n" +
                                             "4. To return the plan, FIRST write a confirmation message, then use the [GENERATE_WORKOUT] tag followed by the JSON object.\n" +
                                             "5. JSON Format: { \"PlanName\": \"...\", \"Goal\": \"...\", \"Difficulty\": \"...\", \"Days\": [ { \"DayName\": \"Monday\", \"Exercises\": [ { \"Name\": \"Exact Name From List\", \"Sets\": 3, \"Reps\": 12, \"Notes\": \"...\" } ] } ] }\n" +
                                             "6. IMPORTANT: Do not use markdown code blocks inside the [GENERATE_WORKOUT] tag. Just raw JSON.";
                }

                // ** YENİ: TARGET WEIGHT AUTOMATION **
                systemInstructionText += "\n\nTARGET WEIGHT RULE:\n" +
                                         "If the user explicitly states a target weight (e.g., 'I want to be 70kg', 'Hedefim 80 kiloya düşmek'), " +
                                         "you MUST include the tag [SET_TARGET_WEIGHT:number] at the end of your response. " +
                                         "Example: 'Harika, hedefini güncelliyorum. [SET_TARGET_WEIGHT:70]'";

                var payload = new
                {
                    system_instruction = new
                    {
                        parts = new[] { new { text = systemInstructionText } }
                    },
                    contents = contents.ToArray()
                };

                // 4. API Çağrısı
                var response = await _httpClient.PostAsJsonAsync(
                    $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}",
                    payload
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"GOOGLE ERROR DETAIL: {errorJson}");
                    return new BaseResponse<AIChatResponse> { Success = false, Message = "Google error: " + response.StatusCode };
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                string aiText;
                try
                {
                    aiText = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString() ?? "I cannot generate a clear response right now.";
                }
                catch
                {
                    aiText = "Response could not be generated.";
                }

                // 5. AI cevabını kaydet
                var aiHistory = new ChatHistory
                {
                    UserId = userId,
                    Role = "model",
                    Message = aiText,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ChatHistories.Add(aiHistory);
                await _context.SaveChangesAsync();

                return new BaseResponse<AIChatResponse>
                {
                    Success = true,
                    Data = new AIChatResponse { Reply = aiText }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AIChatResponse> { Success = false, Message = "An error occurred: " + ex.Message };
            }
        }

        public async Task<BaseResponse<WeeklyDietPlan>> GenerateWeeklyDietPlan()
        {
            try
            {
                var userId = _currentUserService.UserId;
                var lastBmi = await _bmiService.GetLatestBmiByUserId();

                // 1. Get Chat History for Context
                var history = await _context.ChatHistories
                     .Where(x => x.UserId == userId)
                     .OrderByDescending(x => x.CreatedAt)
                     /* Take last 20 messages to ensure we capture the preferences discussed */
                     .Take(20)
                     .OrderBy(x => x.CreatedAt)
                     .ToListAsync();

                // Get Weight History
                var weightHistory = await _context.WeightHistories
                    .Where(x => x.UserId == userId)
                    .OrderBy(x => x.Date)
                    .ToListAsync();
                
                double startWeight = weightHistory.FirstOrDefault()?.Weight ?? (await _context.Users.FindAsync(userId))?.Weight ?? 0;
                double currentWeight = await _context.Users.Where(u => u.Id == userId).Select(u => u.Weight).FirstOrDefaultAsync();
                double targetWeight = await _context.Users.Where(u => u.Id == userId).Select(u => u.TargetWeight).FirstOrDefaultAsync();
                
                // If target weight is 0 in DB, try to find it in chat history or use BMI logic
                string weightProgression = $"Start: {startWeight}kg, Current: {currentWeight}kg, Target: {targetWeight}kg";

                string conversationContext = string.Join("\n", history.Select(h => $"{h.Role}: {h.Message}"));

                string dietPrompt = $"BMI: {lastBmi?.BmiResult ?? 25}.\n" +
                            $"Weight Progression: {weightProgression}\n" +
                            $"Recent Conversation History (User Preferences):\n{conversationContext}\n\n" +
                            "Based on the user's BMI, Weight Progression, and preferences above:\n" +
                            "1. If weight is decreasing, encourage and adjust calories slightly if needed.\n" +
                            "2. Prepare a 7-day diet plan consistent with the goal.\n" +
                            "Output ONLY JSON. " +
                            "JSON format must match this EXACT structure with NO markdown formatting: " +
                            "{ \"Days\": [ { \"DayName\": \"Monday\", \"Breakfast\": \"...\", \"Lunch\": \"...\", \"Dinner\": \"...\", \"Snacks\": \"...\" } ] }";

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
                    return new BaseResponse<WeeklyDietPlan> { Success = false, Message = "Gemini API Error" };

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
                return new BaseResponse<WeeklyDietPlan> { Success = false, Message = "Diet could not be created: " + ex.Message };
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
                    return new BaseResponse<WeeklyDietPlan> { Success = false, Message = "No diet plan created yet." };

                    return new BaseResponse<WeeklyDietPlan> { Success = true, Data = dietPlan };
            }
            catch (Exception)
            {
                return new BaseResponse<WeeklyDietPlan> { Success = false, Message = "Error occurred while fetching diet plan." };
            }
        }
        public async Task<BaseResponse<List<ChatHistoryDto>>> GetChatHistory()
        {
            try
            {
                var userId = _currentUserService.UserId;
                var history = await _context.ChatHistories
                    .Where(x => x.UserId == userId)
                    .OrderBy(x => x.CreatedAt) // Oldest first
                    .Select(x => new ChatHistoryDto
                    {
                        Message = x.Message,
                        IsUser = x.Role == "user",
                        Timestamp = x.CreatedAt
                    })
                    .ToListAsync();

                return new BaseResponse<List<ChatHistoryDto>> { Success = true, Data = history };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<ChatHistoryDto>> { Success = false, Message = "Could not retrieve history: " + ex.Message };
            }
        }
    // ... (Existing Methods)

        public async Task<BaseResponse<Models.Trainings.WorkoutPlan>> GenerateWorkoutPlan(string goal, int days, string level)
        {
            // Bu metod şimdilik boş, asıl mantığı GetAiReply içine prompt injection olarak ekleyeceğiz.
            // Ama Controller'dan direkt çağrılmak istenirse diye stub olarak durabilir.
            return new BaseResponse<Models.Trainings.WorkoutPlan> { Success = false, Message = "Use Ask endpoint for this feature." };
        }

        // Helper to get formatted exercise list context
        private async Task<string> GetExerciseContext()
        {
            // Sadece resmi olan egzersizleri çek
            var exercises = await _context.Exercises
                .Where(x => !string.IsNullOrEmpty(x.ImageUrl))
                .Select(x => new { x.Id, x.Name, x.BodyPart, x.ExerciseLevel, x.IsHome })
                .ToListAsync();

            // JSON maliyetli olabilir, basit string listesi yapalım
            // ID: 123 - Name: Squat (Legs, Advanced) [Home: True]
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("AVAILABLE EXERCISES (Use ONLY these IDs):");
            foreach(var ex in exercises)
            {
                sb.AppendLine($"- ID: {ex.Id} | Name: {ex.Name} | Part: {ex.BodyPart} | Lvl: {ex.ExerciseLevel} | Home: {ex.IsHome}");
            }
            return sb.ToString();
        }

        // ... (Existing GetAiReply Logic override below) ...
    }
}
