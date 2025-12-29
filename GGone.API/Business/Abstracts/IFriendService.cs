using GGone.API.Models;
using GGone.API.Models.Friends;

namespace GGone.API.Business.Abstracts
{
    public interface IFriendService
    {
        Task<BaseResponse<List<FriendResponse>>> GetMyFriends();
        Task<BaseResponse<List<FriendResponse>>> SearchUsers(string query);
        Task<BaseResponse<string>> SendFriendRequest(int friendId);
        Task<BaseResponse<string>> AcceptFriendRequest(int senderId);
    }
}
