using GGone.API.Models.BMI;

namespace GGone.API.Business.Abstracts
{
    public interface IBmiService
    {
        Task<BmiResponse> CalculateAndSaveAsync(CreateBmiRequest request);
        Task<BmiResponse?> GetLatestBmiByUserId(int userId);
    }
}
