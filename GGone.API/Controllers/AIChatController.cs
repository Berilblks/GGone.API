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

        [HttpGet("History")]
        public async Task<BaseResponse<List<ChatHistoryDto>>> GetHistory()
        {
            return await _aiChatService.GetChatHistory();
        }
    }
}
