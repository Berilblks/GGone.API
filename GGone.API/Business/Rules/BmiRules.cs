using GGone.API.Models.Enum;

namespace GGone.API.Business.Rules
{
    public static class BmiRules
    {
        public static BmiStatus GetStatus(double bmi)
        {
            return bmi switch
            {
                < 18.5 => BmiStatus.Underweight,
                < 25 => BmiStatus.NormalWeight,
                < 30 => BmiStatus.Overweight,
                _ => BmiStatus.Obese
            };
        }
    }
}
