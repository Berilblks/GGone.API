using GGone.API.Business.Abstracts;
using GGone.API.Data;
using GGone.API.Models;
using GGone.API.Models.Badges;
using GGone.API.Models.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GGone.API.Business.Services.Badges
{
    public class BadgeService : IBadgeService
    {
        private readonly GGoneDbContext _context;

        // Static definition of all badges
        private static readonly List<BadgeDefinition> _definitions = new()
        {
            // 1. Water
            new() { Id = "water_master", Name = "Pitcher Master", Description = "Complete the 3-liter water goal in a single day.", Icon = "💧", Category = "Water" },
            new() { Id = "water_streak_7", Name = "Waterfall Series", Description = "Reach your water goal for 7 consecutive days.", Icon = "🌊", Category = "Water" },
            new() { Id = "water_100l", Name = "Body Architect", Description = "Reach a total of 100 liters of water consumption.", Icon = "🧊", Category = "Water" },

            // 2. Diet
            new() { Id = "diet_stick", Name = "Green Flag", Description = "Complete the 'Stick to Your Diet' task for the first time.", Icon = "🥗", Category = "Diet" },
            new() { Id = "no_fast_food_3", Name = "Junk Food Buster", Description = "Consume no fast food for 3 consecutive days.", Icon = "🛡️", Category = "Diet" },
            new() { Id = "no_sugar_5", Name = "Sugar Detective", Description = "Complete the 'No Sugary Drinks' task for 5 consecutive days.", Icon = "🚫🥤", Category = "Diet" },
            new() { Id = "cook_hero_10", Name = "Kitchen Hero", Description = "Prepare your own healthy meal 10 times.", Icon = "👨‍🍳", Category = "Diet" },

            // 3. Sports
            new() { Id = "steps_10k", Name = "City Explorer", Description = "Walk 10,000 steps in a single day.", Icon = "👟", Category = "Sports" },
            new() { Id = "marathon_42k", Name = "Marathon Runner", Description = "Reach a total distance of 42 km (approx. 60k steps).", Icon = "🏅", Category = "Sports" },
            new() { Id = "strength_first", Name = "Iron Wrist", Description = "Complete your first 'Strength Training' task.", Icon = "🏋️‍♂️", Category = "Sports" },
            new() { Id = "cardio_streak_3", Name = "Heart Friendly", Description = "Complete the 30-minute cardio task for 3 consecutive days.", Icon = "❤️", Category = "Sports" },

            // 4. Sleep & Detox
            new() { Id = "sleep_streak_5", Name = "Not a Night Owl", Description = "Stick to the 'Sleep Duration' task for 5 consecutive days.", Icon = "😴", Category = "Sleep" },
            new() { Id = "detox_streak_3", Name = "Digital Break", Description = "Successfully complete the 'Phone Detox' task for 3 consecutive days.", Icon = "📵", Category = "Sleep" },
            
            // 5. Milestones
            new() { Id = "all_tasks_11", Name = "11 for 11", Description = "Complete all tasks in a single day without missing any.", Icon = "🌟", Category = "Milestone" },
            new() { Id = "active_14_days", Name = "Belly Warrior", Description = "Stay active in the app for your first 2 weeks (14 days).", Icon = "⚔️", Category = "Milestone" },
            new() { Id = "high_performance_week", Name = "Weekly Consistency", Description = "Achieve at least 80% of your daily goals for a week.", Icon = "📈", Category = "Milestone" },

            // 6. Social
            new() { Id = "share_first", Name = "Source of Inspiration", Description = "Share your success on social media for the first time.", Icon = "📢", Category = "Social" },
            
            // 7. Loyalty
            new() { Id = "loyal_1_month", Name = "Loyal Friend", Description = "It's been 1 month since you downloaded the app.", Icon = "🗓️", Category = "Loyalty" },

            // 8. Interaction
             new() { Id = "profile_architect", Name = "Profile Architect", Description = "Add a profile photo and fill in all your details.", Icon = "👤", Category = "Interaction" },

             // 9. Motivation
             new() { Id = "phoenix_return", Name = "Phoenix", Description = "Complete a task after returning to the app following a 1-week break.", Icon = "🔥", Category = "Motivation" }
        };

        public BadgeService(GGoneDbContext context)
        {
            _context = context;
        }

        public List<BadgeDefinition> GetAllBadgeDefinitions() => _definitions;

        public async Task<BaseResponse<List<UserBadge>>> GetUserBadges(int userId)
        {
            var userBadges = await _context.UserBadges.Where(b => b.UserId == userId).ToListAsync();
            return BaseResponse<List<UserBadge>>.Ok(userBadges);
        }

        public async Task<BaseResponse<List<BadgeResponse>>> GetAllBadgesStatus(int userId)
        {
            var userBadges = await _context.UserBadges.Where(b => b.UserId == userId).ToListAsync();
            
            var response = new List<BadgeResponse>();

            foreach (var def in _definitions)
            {
                var earnedBadge = userBadges.FirstOrDefault(ub => ub.BadgeId == def.Id);
                response.Add(new BadgeResponse
                {
                    Id = def.Id,
                    Name = def.Name,
                    Description = def.Description,
                    Icon = def.Icon,
                    Category = def.Category,
                    IsEarned = earnedBadge != null,
                    EarnedDate = earnedBadge?.EarnedDate
                });
            }

            return BaseResponse<List<BadgeResponse>>.Ok(response);
        }

        public async Task<List<UserBadge>> CheckAndAwardBadges(int userId)
        {
            var newBadges = new List<UserBadge>();
            var existingBadgeIds = await _context.UserBadges
                .Where(b => b.UserId == userId)
                .Select(b => b.BadgeId)
                .ToListAsync();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return newBadges;

            // 1. Fetch Logs
            var taskLogs = await _context.DailyTaskLogs
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.Date)
                .Take(60)
                .ToListAsync();

            // 2. Build Task ID Lookup (String -> Int)
            // We need to know which int ID corresponds to "nut_water", "phys_steps" etc.
            var relevantTaskIds = new[] { 
                "nut_water", "nut_diet", "nut_fastfood", "nut_sugar", 
                "phys_steps", "phys_strength", "phys_cardio",
                "sleep_duration", "sleep_detox"
            };

            var taskMap = await _context.TaskItems
                .Where(t => relevantTaskIds.Contains(t.TaskId))
                .ToDictionaryAsync(t => t.TaskId, t => t.Id);


            // --- Evaluation Helpers ---
            bool Completed(string key) => taskMap.ContainsKey(key) && taskLogs.Any(l => l.CompletedTaskIds.Contains(taskMap[key]));
            
            bool Streak(string key, int days) 
            {
                if (!taskMap.ContainsKey(key)) return false;
                int targetId = taskMap[key];
                int streak = 0;
                // Logs are ordered descending (Today -> Past)
                
                DateTime? lastDate = null;
                foreach (var log in taskLogs)
                {
                    if (log.CompletedTaskIds.Contains(targetId))
                    {
                        // Check continuity
                        if (lastDate == null || (lastDate.Value.Date - log.Date.Date).TotalDays == 1)
                        {
                            streak++;
                            lastDate = log.Date;
                        }
                        else if ((lastDate.Value.Date - log.Date.Date).TotalDays > 1)
                        {
                            break; // Gap found
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                return streak >= days;
            }

            int Count(string key)
            {
                if (!taskMap.ContainsKey(key)) return 0;
                int targetId = taskMap[key];
                return taskLogs.Count(l => l.CompletedTaskIds.Contains(targetId));
            }


            // --- 1. Water ---
            if (!existingBadgeIds.Contains("water_master") && Completed("nut_water")) 
                newBadges.Add(Award(userId, "water_master"));

            if (!existingBadgeIds.Contains("water_streak_7") && Streak("nut_water", 7))
                newBadges.Add(Award(userId, "water_streak_7"));
            
            if (!existingBadgeIds.Contains("water_100l") && Count("nut_water") >= 34)
                newBadges.Add(Award(userId, "water_100l"));

            // --- 2. Diet ---
            if (!existingBadgeIds.Contains("diet_stick") && Completed("nut_diet"))
                 newBadges.Add(Award(userId, "diet_stick"));

             if (!existingBadgeIds.Contains("no_fast_food_3") && Streak("nut_fastfood", 3))
                 newBadges.Add(Award(userId, "no_fast_food_3"));

             if (!existingBadgeIds.Contains("no_sugar_5") && Streak("nut_sugar", 5))
                 newBadges.Add(Award(userId, "no_sugar_5"));
            
            // Note: 'cook_meal' (Kitchen Hero) is NOT in seed data. Logic kept but won't trigger unless TaskId exists.
            
            // --- 3. Sports ---
            if (!existingBadgeIds.Contains("steps_10k") && Completed("phys_steps"))
                 newBadges.Add(Award(userId, "steps_10k"));
            
            // Marathon: approx 6x 10k steps
            if (!existingBadgeIds.Contains("marathon_42k") && Count("phys_steps") >= 6)
                 newBadges.Add(Award(userId, "marathon_42k"));

             if (!existingBadgeIds.Contains("strength_first") && Completed("phys_strength"))
                 newBadges.Add(Award(userId, "strength_first"));

            if (!existingBadgeIds.Contains("cardio_streak_3") && Streak("phys_cardio", 3))
                 newBadges.Add(Award(userId, "cardio_streak_3"));


            // --- 4. Sleep ---
            if (!existingBadgeIds.Contains("sleep_streak_5") && Streak("sleep_duration", 5))
                 newBadges.Add(Award(userId, "sleep_streak_5"));

            if (!existingBadgeIds.Contains("detox_streak_3") && Streak("sleep_detox", 3))
                 newBadges.Add(Award(userId, "detox_streak_3"));


            // --- 7. Loyalty ---
            if (!existingBadgeIds.Contains("loyal_1_month"))
            {
                if (taskLogs.Any() && (DateTime.UtcNow - taskLogs.Last().Date).TotalDays >= 30)
                     newBadges.Add(Award(userId, "loyal_1_month"));
            }

             // --- 8. Interaction ---
            if (!existingBadgeIds.Contains("profile_architect"))
            {
                if (!string.IsNullOrEmpty(user.ProfilePhoto) && user.Height > 0 && user.Weight > 0)
                     newBadges.Add(Award(userId, "profile_architect"));
            }

            // Save new badges
            if (newBadges.Any())
            {
                _context.UserBadges.AddRange(newBadges);
                await _context.SaveChangesAsync();
            }

            return newBadges;
        }

        private UserBadge Award(int userId, string badgeId)
        {
            var def = _definitions.First(d => d.Id == badgeId);
            return new UserBadge
            {
                UserId = userId,
                BadgeId = badgeId,
                Name = def.Name,
                Description = def.Description,
                EarnedDate = DateTime.UtcNow
            };
        }

        // Helpers
        private bool HasCompletedTaskInLogs(List<DailyTaskLog> logs, string taskId)
        {
            // Note: taskLogs checks CompletedTaskIds (int list). We need to map string TaskId to Int ID or assumes logic handles it. 
            // PROBLEM: DailyTaskLog stores List<int> CompletedTaskIds. But badges logic refers to string identifiers (e.g. "drink_water").
            // SOLUTION: We need to retrieve TaskItems to map String ID -> Int ID.
            
            // This requires async calls or loading TaskItems in memory. Doing simple logic for now:
            // Assuming we pass the Integer ID if we knew it. But we only know strings.
            // For now, I'll assume we fetch the ID from Context inside this method (not ideal for perf but works)
            
            // To make this robust, let's fetch IDs first.
            return false; // Stub, will fix in Step 2 with proper lookup.
        }

        private bool CheckStreak(List<DailyTaskLog> logs, string taskId, int days)
        {
             return false; // Stub
        }

        private int CountTaskCompletions(List<DailyTaskLog> logs, string taskId)
        {
            return 0; // Stub
        }
    }
}
