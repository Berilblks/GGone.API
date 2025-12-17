namespace GGone.API.Models.BMI
{
    public class CreateBmiRequest
    {
        public double Height { get; set; } // cm cinsinden
        public double Weight { get; set; } // kg cinsinden
        public int Age { get; set; }
        public required string Gender { get; set; }
    }
}
