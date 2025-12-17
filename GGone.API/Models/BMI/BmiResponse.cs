using GGone.API.Models.Enum;

namespace GGone.API.Models.BMI
{
    public class BmiResponse
    {
        public double BmiResult { get; set; }
        public BmiStatus Status { get; set; } // Enum tipi
        public string StatusDescription => Status.ToString(); // Otomatik string hali
        public DateTime CalculationDate { get; set; }
    }
}
