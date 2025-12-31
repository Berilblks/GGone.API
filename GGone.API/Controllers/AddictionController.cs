using GGone.API.Business.Abstracts;
using GGone.API.Models;
using GGone.API.Models.Addiction;
using GGone.API.Models.Addictions;
using Microsoft.AspNetCore.Mvc;

namespace GGone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class AddictionController : ControllerBase
    {
        private readonly IAddictionService _addictionService;

        public AddictionController(IAddictionService addictionService)
        {
            _addictionService = addictionService;
        }

        [HttpPost("Add")]
        public async Task<BaseResponse<Addiction>> AddUserAddiction(AddAddictionRequest request)
        {
            return await _addictionService.AddUserAddictionAsync(request);
        }

        [HttpGet("Counter")]
        public async Task<BaseResponse<CounterResponse>> GetDependencyCounter([FromQuery] GetCounterRequest request)
        {
            return await _addictionService.GetDependencyCounterAsync(request);
        }

        [HttpPost("QuitDate")]
        public async Task<BaseResponse<object>> QuitDate(QuitDateRequest request)
        {
            return await _addictionService.QuitDateAsync(request);
        }

        [HttpGet("List")]
        public async Task<BaseResponse<List<Addiction>>> GetUserAddictions([FromQuery] GetAddictionRequest request)
        {
            return await _addictionService.GetUserAddictionsAsync(request);
        }

        [HttpGet("CheckDailyStatus")]
        public async Task<BaseResponse<bool>> CheckDailyStatus([FromQuery] GetAddictionRequest request)
        {
            return await _addictionService.CheckDailyStatusAsync(request);
        }
    }
}
