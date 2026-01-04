namespace GGone.API.Models.Diets
{
    public class DietStatusResponse
    {
        public string Status { get; set; } // "Active", "WeighInRequired"
        public int DaysLeft { get; set; }
    }
}
