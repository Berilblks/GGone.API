using AutoMapper;
using GGone.API.Business.Abstracts;
using GGone.API.Data;
using GGone.API.Models;
using GGone.API.Models.Friends;
using Microsoft.EntityFrameworkCore;
using System;

namespace GGone.API.Business.Services.Friends
{
    public class FriendService : IFriendService
    {
        private readonly GGoneDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public FriendService(GGoneDbContext context, IMapper mapper, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<BaseResponse<string>> AcceptFriendRequest(int senderId)
        {
            var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => f.UserId == senderId && f.FriendId == _currentUserService.UserId);

            if (friendship != null)
            {
                friendship.IsAccepted = true;
                await _context.SaveChangesAsync();
            }
            return new BaseResponse<string> { Message = "Artık arkadaşsınız!", Success = true };
        }
        

        public async Task<BaseResponse<List<FriendResponse>>> GetMyFriends()
        {
            var userId = _currentUserService.UserId;

            var friendUsers = await _context.Friendships
               .Include(f => f.User)
               .Include(f => f.Friend)
               .Where(f =>
                   (f.UserId == userId || f.FriendId == userId) &&
                   f.IsAccepted)
               .Select(f => f.UserId == userId ? f.Friend : f.User)
               .ToListAsync();

            var response = _mapper.Map<List<FriendResponse>>(friendUsers);
            return new BaseResponse<List<FriendResponse>> { Data = response, Success = true };
        }

        public async Task<BaseResponse<List<FriendResponse>>> SearchUsers(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new BaseResponse<List<FriendResponse>>
                {
                    Data = new List<FriendResponse>(),
                    Success = true
                };
            }

            var currentUserId = _currentUserService.UserId;

            var users = await _context.Users
                .Where(u =>
                    u.Id != currentUserId &&
                (
                    (!string.IsNullOrEmpty(u.FullName) && u.FullName.Contains(query))
                )
                )
                .Take(10)
                .ToListAsync();

            var response = _mapper.Map<List<FriendResponse>>(users);
            return new BaseResponse<List<FriendResponse>> { Data = response, Success = true };
        }
        

        public async Task<BaseResponse<string>> SendFriendRequest(int friendId)
        {
            var request = new Friendship { UserId = _currentUserService.UserId, FriendId = friendId };
            _context.Friendships.Add(request);
            await _context.SaveChangesAsync();
            return new BaseResponse<string> { Message = "İstek gönderildi", Success = true };
        }
    }
}
