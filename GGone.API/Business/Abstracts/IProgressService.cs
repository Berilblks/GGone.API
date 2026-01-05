using GGone.API.Models;
using GGone.API.Models.Progress;

namespace GGone.API.Business.Abstracts
{
    public interface IProgressService
    {
        Task<BaseResponse<ProgressOverviewResponse>> GetProgressOverview();
    }
}
