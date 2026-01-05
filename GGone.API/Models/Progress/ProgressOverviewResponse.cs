using GGone.API.Models.BMI;

namespace GGone.API.Models.Progress
{
    public class ProgressOverviewResponse
    {
        // Kilo Takibi
        public double CurrentWeight { get; set; }
        public double StartWeight { get; set; }
        public double TargetWeight { get; set; }
        public double WeightLost { get; set; } // Verilen kilo ( pozitif ise kilo vermiş, negatif ise almış)
        public double RemainingWeight { get; set; } // Hedefe kalan
        public double ProgressPercentage { get; set; } // % kaç tamamlandı (0-100)

        // BMI Takibi
        public double CurrentBmi { get; set; }
        public string BmiStatus { get; set; } = string.Empty; // Normal, Overweight vs.
        
        // Grafikler için Tarihçe
        public required List<WeightRecordDto> History { get; set; }

        // Alışkanlık/Streak
        public int CurrentStreak { get; set; }
    }

    public class WeightRecordDto
    {
        public DateTime Date { get; set; }
        public double Weight { get; set; }
        public double Bmi { get; set; }
    }
}
