using System.ComponentModel.DataAnnotations;

namespace GGone.API.Models.Auth
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordExpires { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public int ActiveDays { get; set; }
        public DateOnly LastLoginDate { get; set; }
    }
}
