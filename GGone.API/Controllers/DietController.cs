using GGone.API.Data;
using GGone.API.Models;
using GGone.API.Models.Diets;
using GGone.API.Models.Progress;
using GGone.API.Business.Abstracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GGone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class DietController : ControllerBase
    {
        private readonly GGoneDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public DietController(GGoneDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        [HttpGet("CheckStatus")]
        public async Task<BaseResponse<DietStatusResponse>> CheckStatus()
        {
            try
            {
                var userId = _currentUserService.UserId;
                var activePlan = await _context.WeeklyDietPlans
                    .Where(x => x.UserId == userId && x.IsActive)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();

                if (activePlan == null)
                {
                     // No active plan, maybe logic determines if they can create one? 
                     // For now, let's say Active if no plan exists or handle as special case.
                     // But user likely wants to know if they need to weigh in for an EXISTING plan.
                     return new BaseResponse<DietStatusResponse> 
                     { 
                         Success = true, 
                         Data = new DietStatusResponse { Status = "NoActivePlan", DaysLeft = 0 } 
                     };
                }

                var daysSinceCreation = (DateTime.UtcNow - activePlan.CreatedAt).TotalDays;
                
                // If 7 days have passed (e.g. >= 7.0), weigh-in is required
                if (daysSinceCreation >= 7)
                {
                    return new BaseResponse<DietStatusResponse> 
                    { 
                        Success = true, 
                        Data = new DietStatusResponse { Status = "WeighInRequired", DaysLeft = 0 } 
                    };
                }
                else
                {
                    return new BaseResponse<DietStatusResponse> 
                    { 
                        Success = true, 
                        Data = new DietStatusResponse 
                        { 
                            Status = "Active", 
                            DaysLeft = 7 - (int)daysSinceCreation 
                        } 
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse<DietStatusResponse> 
                { 
                    Success = false, 
                    Message = "Error checking status: " + ex.Message 
                };
            }
        }
    }
}
