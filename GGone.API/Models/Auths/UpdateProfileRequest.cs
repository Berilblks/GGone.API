using System.ComponentModel.DataAnnotations;

namespace GGone.API.Models.Auth
{
    public class UpdateProfileRequest
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
        
        public string? ProfilePhoto { get; set; }
    }
}
