using System.ComponentModel.DataAnnotations;

namespace GGone.API.Models.Auth
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public DateOnly BirthDate { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public double TargetWeight { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordExpires { get; set; }
        public string? ProfilePhoto { get; set; }
        public int ActiveDays { get; set; }
        public DateOnly LastLoginDate { get; set; }

        public string? DeleteAccountToken { get; set; }

        public DateTime? DeleteAccountExpires { get; set; }

        // Leveling System
        public int Level { get; set; } = 1;
        public int CurrentXp { get; set; } = 0;
    }
}
