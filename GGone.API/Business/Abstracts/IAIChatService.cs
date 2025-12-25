using GGone.API.Models;
using GGone.API.Models.AI;

namespace GGone.API.Business.Abstracts
{
    public interface IAIChatService
    {
        Task<BaseResponse<AIChatResponse>> GetAiReply(AIChatRequest request);
    }
}
