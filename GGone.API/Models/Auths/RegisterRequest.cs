using System.ComponentModel.DataAnnotations;

namespace GGone.API.Models.Auth
{
    public class RegisterRequest
    {
        public required string FullName { get; set; }
        public required string Username { get; set; }
        public required int BirthDay { get; set; }
        public required int BirthMonth { get; set; }
        public required int BirthYear { get; set; }
        public required double Height { get; set; }
        public required double Weight { get; set; }
        
        [AllowedValues("Woman", "Man", ErrorMessage = "Gender must be either 'Woman' or 'Man'.")]
        public required string Gender { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required double TargetWeight { get; set; }
    }
}
