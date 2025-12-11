using GGone.API.Models.Enum;

namespace GGone.API.Models.Addictions
{
    public class AddAddictionRequest
    {
        public int UserId { get; set; }
        public AddictionType Type { get; set; }
        public double DailyConsumption { get; set; }
        public double UnitPrice { get; set; }
        public DateTime QuitDate { get; set; }
    }
}
