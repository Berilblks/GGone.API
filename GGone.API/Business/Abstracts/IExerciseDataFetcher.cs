using GGone.API.Models;

namespace GGone.API.Business.Abstracts
{
    public interface IExerciseDataFetcher
    {
        Task<BaseResponse<string>> FetchAndSaveAllExercises();
    }
}
