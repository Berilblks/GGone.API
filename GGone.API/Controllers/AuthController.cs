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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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

    }
}
