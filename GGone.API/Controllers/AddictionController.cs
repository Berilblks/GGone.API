using GGone.API.Business.Abstracts;
using GGone.API.Models;
using GGone.API.Models.Addiction;
using GGone.API.Models.Addictions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GGone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddictionController : ControllerBase
    {
        private readonly IAddictionService _addictionService;

        public AddictionController(IAddictionService addictionService)
        {
            _addictionService = addictionService;
        }

        private int GetUserIdFromClaims()
        {
            return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) ? userId : 0; 
        }

        [HttpPost("Add")]
        public async Task<BaseResponse<Addiction>> AddUserAddiction(AddAddictionRequest request)
        {
            request.UserId = GetUserIdFromClaims();
            return await _addictionService.AddUserAddictionAsync(request);
        }

        [HttpGet("Counter")]
        public async Task<BaseResponse<CounterResponse>> GetDependencyCounter([FromQuery] GetCounterRequest request)
        {
            request.UserId = GetUserIdFromClaims();
            return await _addictionService.GetDependencyCounterAsync(request);
        }

        [HttpPost("QuitDate")]
        public async Task<BaseResponse<object>> QuitDate(QuitDateRequest request)
        {
            request.UserId = GetUserIdFromClaims();
            return await _addictionService.QuitDateAsync(request);
        }

        [HttpGet("List")]
        public async Task<BaseResponse<List<Addiction>>> GetUserAddictions([FromQuery] GetAddictionRequest request)
        {
            request.UserId = GetUserIdFromClaims();
            return await _addictionService.GetUserAddictionsAsync(request);
        }

        [HttpGet("CheckDailyStatus")]
        public async Task<BaseResponse<bool>> CheckDailyStatus([FromQuery] GetAddictionRequest request)
        {
            request.UserId = GetUserIdFromClaims();
            return await _addictionService.CheckDailyStatusAsync(request);
        }
    }
}
