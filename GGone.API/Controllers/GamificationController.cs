using GGone.API.Business.Abstracts;
using GGone.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GGone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GamificationController : ControllerBase
    {
        private readonly ILevelService _levelService;
        private readonly ICurrentUserService _currentUserService;

        public GamificationController(ILevelService levelService, ICurrentUserService currentUserService)
        {
            _levelService = levelService;
            _currentUserService = currentUserService;
        }

        [HttpGet("Progress")]
        public async Task<BaseResponse<LevelProgressResponse>> GetProgress()
        {
            return await _levelService.GetUserLevelProgress(_currentUserService.UserId);
        }
    }
}
