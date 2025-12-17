using GGone.API.Models.Enum;

namespace GGone.API.Models.BMI
{
    public static class BmiCalculatorHelper
    {
        public static BmiStatus GetStatus(double bmi)
        {
            return bmi switch
            {
                < 18.5 => BmiStatus.Underweight,
                < 24.9 => BmiStatus.NormalWeight,
                < 29.9 => BmiStatus.Overweight,
                < 34.9 => BmiStatus.Obese,
            };
        }
    }
}
