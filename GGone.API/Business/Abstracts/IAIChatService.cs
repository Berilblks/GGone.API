using GGone.API.Models;
using GGone.API.Models.AI;
using GGone.API.Models.Diets;

namespace GGone.API.Business.Abstracts
{
    public interface IAIChatService
    {
        Task<BaseResponse<AIChatResponse>> GetAiReply(AIChatRequest request);
        Task<BaseResponse<WeeklyDietPlan>> GenerateWeeklyDietPlan();
        Task<BaseResponse<WeeklyDietPlan>> GetUserDietPlan();
    }
}
