using GGone.API.Business.Abstracts;
using GGone.API.Models;
using GGone.API.Models.Badges;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GGone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BadgeController : ControllerBase
    {
        private readonly IBadgeService _badgeService;
        private readonly ICurrentUserService _currentUserService;

        public BadgeController(IBadgeService badgeService, ICurrentUserService currentUserService)
        {
            _badgeService = badgeService;
            _currentUserService = currentUserService;
        }

        [HttpGet("MyBadges")]
        public async Task<BaseResponse<List<UserBadge>>> GetMyBadges()
        {
            var userId = _currentUserService.UserId;
            
            // First, check and award any new badges
            await _badgeService.CheckAndAwardBadges(userId);

            // Then return the full list
            return await _badgeService.GetUserBadges(userId);
        }

        [HttpGet("GetAll")]
        public async Task<BaseResponse<List<BadgeResponse>>> GetAll()
        {
            var userId = _currentUserService.UserId;

            // Trigger check to ensure updated status
            await _badgeService.CheckAndAwardBadges(userId);

            return await _badgeService.GetAllBadgesStatus(userId);
        }
    }
}
