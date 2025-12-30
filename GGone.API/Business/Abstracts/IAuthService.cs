using GGone.API.Models;
using GGone.API.Models.Auth;

namespace GGone.API.Business.Abstracts
{
    public interface IAuthService
    {
        Task<BaseResponse<LoginResponse>> Login(LoginRequest request);
        Task<BaseResponse<RegisterResponse>> Register(RegisterRequest request);
        Task<BaseResponse<ChangePasswordResponse>> ChangePassword(ChangePasswordRequest request);
        Task<BaseResponse<ForgetPasswordResponse>> ForgetPassword(ForgetPasswordRequest request);
        Task<BaseResponse<SendVerificationCodeResponse>> SendVerificationCode(SendVerificationCodeRequest request);
        Task<BaseResponse<ResetPasswordResponse>> ResetPassword(ResetPasswordRequest request);
    }
}
