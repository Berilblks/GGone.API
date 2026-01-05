using GGone.API.Business.Abstracts;
using GGone.API.Models;
using GGone.API.Models.Friends;
using Microsoft.AspNetCore.Mvc;

namespace GGone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService _friendService;

        public FriendController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        [HttpGet("List")]
        public async Task<BaseResponse<List<FriendResponse>>> GetFriends()
        {
            return await _friendService.GetMyFriends();
        }

        [HttpGet("Search")]
        public async Task<BaseResponse<List<FriendResponse>>> Search([FromQuery] string? q)
        {
            return await _friendService.SearchUsers(q);
        }

        [HttpPost("Request")]
        public async Task<BaseResponse<string>> RequestFriend(int friendId)
        {
            return await _friendService.SendFriendRequest(friendId);
        }

        [HttpPost("Accept")]
        public async Task<BaseResponse<string>> AcceptFriend(int senderId)
        {
            return await _friendService.AcceptFriendRequest(senderId);
        }
    }
}