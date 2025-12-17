namespace GGone.API.Models.BMI
{
    public class CreateBmiRecordDto
    {
        public double Height { get; set; }
        public double Weight { get; set; }
        public int Age { get; set; }
        public required string Gender { get; set; }
    }
}
