namespace GGone.API.Models.Auth
{
    public class ResetPasswordRequest
    {
        public required string Email { get; set; }
        public required string Code { get; set; }
        public required string NewPassword { get; set; }
    }
}
