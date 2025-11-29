namespace GGone.API.Models.Auth
{
    public class ForgetPasswordRequest
    {
        public required string Email { get; set; }
        public required string NewPassword { get; set; }
    }
}
