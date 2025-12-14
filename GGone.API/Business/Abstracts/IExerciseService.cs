using GGone.API.Models;
using GGone.API.Models.Exercises;

namespace GGone.API.Business.Abstracts
{
    public interface IExerciseService
    {
        Task<BaseResponse<List<ExerciseResponse>>> GetExercises(ExerciseFilterRequest request);
        Task<BaseResponse<ExerciseResponse>> GetExerciseById(int id);
    }
}
