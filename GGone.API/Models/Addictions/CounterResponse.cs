using GGone.API.Models.Enum;

namespace GGone.API.Models.Addictions
{
    public class CounterResponse
    {
        public int UserId { get; set; }
        public AddictionType Type { get; set; }
        public int DaysClean { get; set; }
        public DateTime QuitDate { get; set; }
        public DateTime LastConsumptionDate { get; set; }

        public double DailyConsumption { get; set; }    
        public double UnitPrice { get; set; }
    }
}
