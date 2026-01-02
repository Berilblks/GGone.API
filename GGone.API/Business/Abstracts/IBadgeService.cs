using GGone.API.Models;
using GGone.API.Models.Badges;

namespace GGone.API.Business.Abstracts
{
    public interface IBadgeService
    {
        /// <summary>
        /// Checks all badge criteria for the user and awards new ones if conditions are met.
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <returns>A list of newly awarded badges.</returns>
        Task<List<UserBadge>> CheckAndAwardBadges(int userId);

        /// <summary>
        /// Gets all badges earned by the user.
        /// </summary>
        Task<BaseResponse<List<UserBadge>>> GetUserBadges(int userId);

        /// <summary>
        /// Gets all possible badges with user's earned status.
        /// </summary>
        Task<BaseResponse<List<BadgeResponse>>> GetAllBadgesStatus(int userId);

        /// <summary>
        /// Gets list of all possible badges (locked and unlocked).
        /// </summary>
        List<BadgeDefinition> GetAllBadgeDefinitions();
    }
}
