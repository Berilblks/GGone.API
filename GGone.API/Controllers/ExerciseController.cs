using GGone.API.Business.Abstracts;
using GGone.API.Models;
using GGone.API.Models.Exercises;
using Microsoft.AspNetCore.Mvc;

namespace GGone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;
        private readonly IExerciseDataFetcher _exerciseDataFetcher;

        public ExerciseController(IExerciseService exerciseService, IExerciseDataFetcher exerciseDataFetcher)
        {
            _exerciseService = exerciseService;
            _exerciseDataFetcher = exerciseDataFetcher;
        }

        [HttpGet("GetExercises")]
        public async Task<BaseResponse<List<ExerciseResponse>>> GetExercises([FromQuery] ExerciseFilterRequest request)
        {
            return await _exerciseService.GetExercises(request);
        }

        [HttpGet("GetExerciseById")]
        public async Task<BaseResponse<ExerciseResponse>> GetExerciseById(int id)
        {
            return await _exerciseService.GetExerciseById(id);
        }

        [HttpPost("FetchAll")]
        public async Task<BaseResponse<string>> FetchAll()
        {
            return await _exerciseDataFetcher.FetchAndSaveAllExercises();
        }

    }
}
