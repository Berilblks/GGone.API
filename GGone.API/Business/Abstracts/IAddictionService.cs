using GGone.API.Models;
using GGone.API.Models.Addiction;
using GGone.API.Models.Addictions;


namespace GGone.API.Business.Abstracts
{
    public interface IAddictionService
    {
        // Add a new addiction for a user
        Task<BaseResponse<Addiction>> AddUserAddictionAsync(AddAddictionRequest request);

        // List all addictions for a user
        Task<BaseResponse<List<Addiction>>> GetUserAddictionsAsync(GetAddictionRequest request);

        // Record a daily addiction status
        Task<BaseResponse<object>> QuitDateAsync(QuitDateRequest request);

        // Counter for addictions
        Task<BaseResponse<CounterResponse>> GetDependencyCounterAsync(GetCounterRequest request);

        // Günlük durum kontrolü
        Task<BaseResponse<bool>> CheckDailyStatusAsync(GetAddictionRequest request);
    }
}
