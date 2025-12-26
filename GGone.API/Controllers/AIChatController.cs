using GGone.API.Business.Abstracts;
using GGone.API.Models;
using GGone.API.Models.AI;
using GGone.API.Models.Diets;
using Microsoft.AspNetCore.Mvc;

namespace GGone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIChatController : ControllerBase
    {
        private readonly IAIChatService _aiChatService;

        public AIChatController(IAIChatService aiChatService)
        {
            _aiChatService = aiChatService;
        }

        [HttpPost("Ask")]
        public async Task<BaseResponse<AIChatResponse>> Ask(AIChatRequest request)
        {
            return await _aiChatService.GetAiReply(request);
        }

        [HttpPost("GenerateWeeklyDietPlan")]
        public async Task<BaseResponse<WeeklyDietPlan>> GenerateWeeklyDietPlan()
        {
            return await _aiChatService.GenerateWeeklyDietPlan();
        }

        [HttpGet("GetUserDietPlan")]
        public async Task<BaseResponse<WeeklyDietPlan>> GetUserDietPlan()
        {
            return await _aiChatService.GetUserDietPlan();
        }
    }
}
