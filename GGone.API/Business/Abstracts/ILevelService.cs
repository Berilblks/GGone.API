using GGone.API.Models;

namespace GGone.API.Business.Abstracts
{
    public interface ILevelService
    {
        Task<BaseResponse<string>> AddXp(int userId, int amount, string reason);
        Task<BaseResponse<LevelProgressResponse>> GetUserLevelProgress(int userId);
    }

    public class LevelProgressResponse
    {
        public int Level { get; set; }
        public int CurrentXp { get; set; }
        public int XpForNextLevel { get; set; }
        public double ProgressPercentage { get; set; }
    }
}
