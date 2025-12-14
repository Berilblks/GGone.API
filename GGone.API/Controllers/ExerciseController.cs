using GGone.API.Business.Abstracts;
using GGone.API.Models;
using GGone.API.Models.Exercises;
using Microsoft.AspNetCore.Mvc;

namespace GGone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;
        public ExerciseController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
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

    }
}
