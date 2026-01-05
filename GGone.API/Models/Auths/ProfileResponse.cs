namespace GGone.API.Models.Auth
{
    public class ProfileResponse
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public DateOnly BirthDate { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public double TargetWeight { get; set; }
        public string? Gender { get; set; }
        public string? ProfilePhoto { get; set; }
    }
}
