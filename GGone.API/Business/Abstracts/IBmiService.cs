using GGone.API.Models.BMI;

namespace GGone.API.Business.Abstracts
{
    public interface IBmiService
    {
        Task<BmiResponse> CalculateAndSaveAsync(CreateBmiRequest request);
        Task<BmiResponse?> GetLatestBmiByUserId();
        Task<List<BmiResponse>> GetBmiHistory();
        Task<int> GetUserStreak();

    }
}
