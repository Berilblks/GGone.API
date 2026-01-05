using AutoMapper;
using GGone.API.Models.Auth;
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
            var currentUserId = _currentUserService.UserId;
            List<User> users;

            if (string.IsNullOrWhiteSpace(query))
            {
                Console.WriteLine($"DEBUG: SearchUsers called with EMPTY query. Fetching suggested users...");
                
                // Query boşsa rastgele/önerilen 20 kişiyi getir (Kendisi hariç, ismi olanlar)
                users = await _context.Users
                    .Where(u => u.Id != currentUserId && !string.IsNullOrEmpty(u.FullName))
                    .OrderByDescending(u => u.LastLoginDate) // Örn: Son aktif olanlar
                    .Take(20)
                    .ToListAsync();
                    
                Console.WriteLine($"DEBUG: Found {users.Count} suggested users.");
            }
            else
            {
                query = query.ToLower();
                // 1. Kullanıcıları bul (Username veya FullName)
                users = await _context.Users
                    .Where(u =>
                        u.Id != currentUserId &&
                    (
                        (!string.IsNullOrEmpty(u.FullName) && u.FullName.ToLower().Contains(query)) ||
                        (!string.IsNullOrEmpty(u.Username) && u.Username.ToLower().Contains(query))
                    ))
                    .Take(20) 
                    .ToListAsync();
            }

            // 2. Bu kullanıcılarla olan ilişki durumunu çek
            var userIds = users.Select(u => u.Id).ToList();
            
            var friendships = await _context.Friendships
                .Where(f => 
                    (f.UserId == currentUserId && userIds.Contains(f.FriendId)) || // Ben ekledim
                    (f.FriendId == currentUserId && userIds.Contains(f.UserId))    // O ekledi
                )
                .ToListAsync();

            // 3. Response map ve status doldurma
            var responseList = new List<FriendResponse>();

            foreach (var user in users)
            {
                var resp = _mapper.Map<FriendResponse>(user);
                
                // İlişki kontrolü
                var existingRel = friendships.FirstOrDefault(f => f.UserId == user.Id || f.FriendId == user.Id);
                
                if (existingRel == null)
                {
                    resp.Status = "None";
                    resp.IsFriend = false;
                }
                else if (existingRel.IsAccepted)
                {
                    resp.Status = "Accepted";
                    resp.IsFriend = true;
                    // Eğer arkadaşsak resim vs düzgün görünsün
                }
                else
                {
                    // Bekleyen istek var. Ama kim kime atmış?
                    if (existingRel.UserId == currentUserId)
                    {
                        resp.Status = "Pending"; // Ben atmışım, bekliyor
                    }
                    else
                    {
                        resp.Status = "Incoming"; // O bana atmış, kabul etmemi bekliyor
                    }
                    resp.IsFriend = false;
                }

                responseList.Add(resp);
            }

            return new BaseResponse<List<FriendResponse>> { Data = responseList, Success = true };
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
