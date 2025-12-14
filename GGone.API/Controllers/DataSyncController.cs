using GGone.API.Business.Abstracts;
using GGone.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace GGone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataSyncController : ControllerBase
    {
        private readonly IExerciseDataFetcher _dataFetcher;

        public DataSyncController(IExerciseDataFetcher dataFetcher)
        {
            _dataFetcher = dataFetcher;
        }

        [HttpPost("SyncExercises")]
        public async Task<BaseResponse<string>> SyncExercises()
        {
            // Tüm işi servise delege etme (Sade Controller)
            return await _dataFetcher.FetchAndSaveAllExercises();
            
        }
    }
}
