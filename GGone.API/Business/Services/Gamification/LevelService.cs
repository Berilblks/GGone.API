using GGone.API.Business.Abstracts;
using GGone.API.Data;
using GGone.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GGone.API.Business.Services.Gamification
{
    public class LevelService : ILevelService
    {
        private readonly GGoneDbContext _context;

        public LevelService(GGoneDbContext context)
        {
            _context = context;
        }

        public async Task<BaseResponse<string>> AddXp(int userId, int amount, string reason)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return new BaseResponse<string> { Success = false, Message = "User not found" };

            // XP Ekle
            user.CurrentXp += amount;
            
            // Level Kontrolü
            bool leveledUp = false;
            while (true)
            {
                int xpNeeded = CalculateXpForNextLevel(user.Level);
                if (user.CurrentXp >= xpNeeded)
                {
                    user.CurrentXp -= xpNeeded;
                    user.Level++;
                    leveledUp = true;
                }
                else
                {
                    break;
                }
            }

            await _context.SaveChangesAsync();

            string msg = leveledUp 
                ? $"Tebrikler! Seviye atladın! Yeni Seviye: {user.Level}" 
                : $"{amount} XP kazanıldı! ({reason})";

            return new BaseResponse<string> { Success = true, Message = msg };
        }

        public async Task<BaseResponse<LevelProgressResponse>> GetUserLevelProgress(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return new BaseResponse<LevelProgressResponse> { Success = false };

            // Migration sonrası Level 0 kalmış olabilir, UI için en az 1 gösterelim.
            int safeLevel = user.Level < 1 ? 1 : user.Level;
            int xpNeeded = CalculateXpForNextLevel(safeLevel);
            
            double percentage = 0;
            if (xpNeeded > 0)
            {
                percentage = (double)user.CurrentXp / xpNeeded;
            }

            // Yüzde 1'i geçmesin (görsel için)
            if (percentage > 1) percentage = 1;

            return new BaseResponse<LevelProgressResponse>
            {
                Success = true,
                Data = new LevelProgressResponse
                {
                    Level = safeLevel, // DB'yi değiştirmeden safeLevel dönüyoruz
                    CurrentXp = user.CurrentXp,
                    XpForNextLevel = xpNeeded,
                    ProgressPercentage = percentage
                }
            };
        }

        private int CalculateXpForNextLevel(int currentLevel)
        {
            // Level 0 veya negatif gelirse 1 gibi işlem görsün
            if (currentLevel < 1) currentLevel = 1;
            
            // Formül: Her seviye için 100 * Seviye kadar XP gerekir.
            return currentLevel * 100;
        }
    }
}
