using GGone.API.Business.Abstracts;
using GGone.API.Models;
using GGone.API.Models.AI;
using GGone.API.Models.Diets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GGone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class AIChatController : ControllerBase
    {
        private readonly IAIChatService _aiChatService;
        private readonly GGone.API.Data.GGoneDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public AIChatController(IAIChatService aiChatService, GGone.API.Data.GGoneDbContext context, ICurrentUserService currentUserService)
        {
            _aiChatService = aiChatService;
            _context = context;
            _currentUserService = currentUserService;
        }

        [HttpPost("Ask")]
        public async Task<BaseResponse<AIChatResponse>> Ask(AIChatRequest request)
        {

            var response = await _aiChatService.GetAiReply(request);

            if (response.Success && response.Data != null && !string.IsNullOrEmpty(response.Data.Reply))
            {
                if (response.Data.Reply.Contains("[GENERATE_DIET]"))
                {
                    try
                    {
                        var content = response.Data.Reply.Replace("[GENERATE_DIET]", "").Trim();
                        var newDiet = new DietPlan
                        {
                            UserId = _currentUserService.UserId,
                            Content = content,
                            CreatedAt = DateTime.UtcNow
                        };
                        _context.DietPlans.Add(newDiet);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        // Log error but don't fail the response
                        Console.WriteLine($"Error saving generated diet: {ex.Message}");
                    }
                }

                // ** YENİ: WORKOUT GENERATION CAPTURE **
                if (response.Data.Reply.Contains("[GENERATE_WORKOUT]"))
                {
                    Console.WriteLine("DEBUG: [GENERATE_WORKOUT] tag DETECTED in AI response."); 
                    try
                    {
                        // 1. Tag'i temizle ve JSON'u al
                        var parts = response.Data.Reply.Split("[GENERATE_WORKOUT]");
                        if (parts.Length < 2) 
                        {
                            Console.WriteLine("DEBUGGING ERROR: Tag found but split failed.");
                        }

                        var rawContent = parts[1].Trim();
                        
                        // JSON bulma mantığı (Daha esnek)
                        int firstBrace = rawContent.IndexOf('{');
                        int lastBrace = rawContent.LastIndexOf('}');
                        
                        if (firstBrace != -1 && lastBrace != -1 && lastBrace > firstBrace)
                        {
                            var jsonCandidate = rawContent.Substring(firstBrace, lastBrace - firstBrace + 1);
                            
                            Console.WriteLine($"DEBUG: Extracted JSON candidate: {jsonCandidate.Substring(0, Math.Min(jsonCandidate.Length, 50))}...");

                            // 2. Deserialize et
                            var workoutPlan = System.Text.Json.JsonSerializer.Deserialize<Models.Trainings.WorkoutPlan>(jsonCandidate, 
                                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                            if (workoutPlan != null)
                            {
                                Console.WriteLine($"DEBUG: Deserialization SUCCESS. Plan Name: {workoutPlan.PlanName}. Saving to DB...");
                                
                                workoutPlan.UserId = _currentUserService.UserId;
                                workoutPlan.CreatedAt = DateTime.UtcNow;
                                workoutPlan.IsActive = true; // FORCE ACTIVE

                                // LINK EXERCISES BY NAME
                                foreach(var day in workoutPlan.Days)
                                {
                                    foreach(var exercise in day.Exercises)
                                    {
                                        var lookupName = exercise.Name.Trim().ToLower();
                                        Console.WriteLine($"DEBUG: Looking up exercise: '{lookupName}'");

                                        // Try to find matching exercise in DB
                                        var dbExercise = await _context.Exercises
                                            .FirstOrDefaultAsync(e => e.Name.Trim().ToLower() == lookupName);
                                        
                                        if (dbExercise != null)
                                        {
                                            exercise.ExerciseId = dbExercise.Id;
                                            Console.WriteLine($"DEBUG: Match FOUND! Linked to ID: {dbExercise.Id} | Image: {dbExercise.ImageUrl}");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"DEBUG: Match FAILED for '{lookupName}'");
                                        }
                                    }
                                }

                                _context.WorkoutPlans.Add(workoutPlan);
                                await _context.SaveChangesAsync();
                                
                                Console.WriteLine($"DEBUG: WorkoutPlan SAVED to Database! Id: {workoutPlan.Id}");
                            }
                            else
                            {
                                Console.WriteLine("DEBUG: Deserialization FAILED. workoutPlan object is null.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("DEBUG: JSON braces '{' and '}' not found in the content after tag.");
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"DEBUG ERROR saving generated workout: {ex.Message} | Stack: {ex.StackTrace}");
                    }
                }

                else
                {
                    Console.WriteLine("DEBUG: [GENERATE_WORKOUT] tag NOT found in AI response.");
                }

                // ** YENİ: TARGET WEIGHT AUTOMATION **
                // Örnek: [SET_TARGET_WEIGHT:75]
                if (response.Data.Reply.Contains("[SET_TARGET_WEIGHT:"))
                {
                    try 
                    {
                        var startTag = "[SET_TARGET_WEIGHT:";
                        int startIndex = response.Data.Reply.IndexOf(startTag) + startTag.Length;
                        int endIndex = response.Data.Reply.IndexOf("]", startIndex);

                        if (startIndex > -1 && endIndex > startIndex)
                        {
                            string weightStr = response.Data.Reply.Substring(startIndex, endIndex - startIndex).Trim();
                            if (double.TryParse(weightStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double targetWeight))
                            {
                                var user = await _context.Users.FindAsync(_currentUserService.UserId);
                                if (user != null)
                                {
                                    user.TargetWeight = targetWeight;
                                    await _context.SaveChangesAsync();
                                    Console.WriteLine($"DEBUG: User TargetWeight updated to {targetWeight}kg via AI.");
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                         Console.WriteLine($"Error updating target weight: {ex.Message}");
                    }
                }
            }

            return response;
        }


        [HttpPost("GenerateWeeklyDietPlan")]
        public async Task<BaseResponse<WeeklyDietPlan>> GenerateWeeklyDietPlan()
        {
            return await _aiChatService.GenerateWeeklyDietPlan();
        }

        [HttpGet("GetUserDietPlan")]
        public async Task<BaseResponse<DietPlan>> GetUserDietPlan()
        {
            var userId = _currentUserService.UserId;
            var plan = await _context.DietPlans
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (plan == null)
            {
                return new BaseResponse<DietPlan> { Success = false, Message = "No diet plan found." };
            }

            return new BaseResponse<DietPlan> { Success = true, Data = plan };
        }

        [HttpGet("GetUserWorkoutPlan")]
        public async Task<IActionResult> GetUserWorkoutPlan()
        {
            var userId = _currentUserService.UserId;
            
            // Get the most recent active plan
            var plan = await _context.WorkoutPlans
                .Include(wp => wp.Days)
                .ThenInclude(d => d.Exercises)
                    .ThenInclude(e => e.Exercise) // <--- INCLUDE ADDED
                .Where(wp => wp.UserId == userId && wp.IsActive)
                .OrderByDescending(wp => wp.CreatedAt)
                .FirstOrDefaultAsync();
                
            if (plan == null)
            {
                return Ok(new { success = false, message = "No active plan found." });
            }
            return Ok(new { success = true, data = plan });
        }

        [HttpGet("History")]
        public async Task<BaseResponse<List<ChatHistoryDto>>> GetHistory()
        {
            return await _aiChatService.GetChatHistory();
        }
    }
}
