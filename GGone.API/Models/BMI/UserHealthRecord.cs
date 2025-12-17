namespace GGone.API.Models.BMI
{
    public class UserHealthRecord
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Hangi kullanıcıya ait olduğu
        public double Height { get; set; }
        public double Weight { get; set; }
        public int Age { get; set; }
        public required string Gender { get; set; }
        public double BmiResult { get; set; } // Hesaplanan VKI
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
