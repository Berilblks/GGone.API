using GGone.API.Models.BMI;

namespace GGone.API.Models.Progress
{
    public class ProgressOverviewResponse
    {
        public int StreakCount { get; set; }
        public required List<BmiResponse> BmiHistory { get; set; }
    }
}
