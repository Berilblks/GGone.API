using GGone.API.Business.Abstracts;
using GGone.API.Models;
using GGone.API.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace GGone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;
        private readonly GGone.API.Data.GGoneDbContext _context;

        public AuthController(IAuthService authService, ICurrentUserService currentUserService, GGone.API.Data.GGoneDbContext context)
        {
            _authService = authService;
            _currentUserService = currentUserService;
            _context = context;
        }

        [HttpPost("UpdateWeight")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<BaseResponse<string>> UpdateWeight(GGone.API.Models.Progress.UpdateWeightRequest request)
        {
            try 
            {
                var userId = _currentUserService.UserId;
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return new BaseResponse<string> { Success = false, Message = "User not found" };

                // 1. Add to history
                var history = new GGone.API.Models.Progress.WeightHistory
                {
                    UserId = userId,
                    Weight = request.NewWeight,
                    Date = DateTime.UtcNow
                };
                _context.WeightHistories.Add(history);

                // 2. Update current weight
                user.Weight = request.NewWeight;
                await _context.SaveChangesAsync();

                return new BaseResponse<string> { Success = true, Message = "Weight updated successfully." };
            }
            catch(Exception ex)
            {
                return new BaseResponse<string> { Success = false, Message = "Update failed: " + ex.Message };
            }
        }

        [HttpPost("Register")]
        public async Task<BaseResponse<RegisterResponse>> Register(RegisterRequest request)
        {
            return await _authService.Register(request);
        }

        [HttpPost("Login")]
        public async Task<BaseResponse<LoginResponse>> Login(LoginRequest request)
        {
            return await _authService.Login(request);

        }

        [HttpPost("ChangePassword")]
        public async Task<BaseResponse<ChangePasswordResponse>> ChangePassword(ChangePasswordRequest request)
        {
            return await _authService.ChangePassword(request);
        }

        [HttpPost("ForgetPassword")]
        public async Task<BaseResponse<ForgetPasswordResponse>> ForgetPassword(ForgetPasswordRequest request)
        {
            return await _authService.ForgetPassword(request);
        }

        [HttpPost("SendVerificationCode")]
        public async Task<BaseResponse<SendVerificationCodeResponse>> SendVerificationCode(SendVerificationCodeRequest request)
        {
            return await _authService.SendVerificationCode(request);
        }

        [HttpPost("ResetPassword")]
        public async Task<BaseResponse<ResetPasswordResponse>> ResetPassword(ResetPasswordRequest request)
        {
            return await _authService.ResetPassword(request);
        }
        [HttpGet("Profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<BaseResponse<ProfileResponse>> GetProfile()
        {
            return await _authService.GetProfile();
        }

        [HttpPut("UpdateProfile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<BaseResponse<ProfileResponse>> UpdateProfile(UpdateProfileRequest request)
        {
            return await _authService.UpdateProfile(request);
        }

        [HttpPost("RequestDeleteAccount")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<BaseResponse<string>> RequestDeleteAccount()
        {
            return await _authService.RequestDeleteAccount();
        }

        [HttpDelete("ConfirmDeleteAccount")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<BaseResponse<string>> ConfirmDeleteAccount(ConfirmDeleteRequest request)
        {
            return await _authService.ConfirmDeleteAccount(request);
        }
    }
}
