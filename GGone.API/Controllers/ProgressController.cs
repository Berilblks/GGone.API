using GGone.API.Business.Abstracts;
using GGone.API.Models;
using GGone.API.Models.Progress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GGone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        [HttpGet("Overview")]
        public async Task<BaseResponse<ProgressOverviewResponse>> GetOverview()
        {
            return await _progressService.GetProgressOverview();
        }
    }
}
