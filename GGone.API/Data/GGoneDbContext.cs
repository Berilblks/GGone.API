using GGone.API.Models.Addiction;
using GGone.API.Models.Auth;
using GGone.API.Models.BMI;
using GGone.API.Models.Diets;
using GGone.API.Models.Exercises;
using GGone.API.Models.Friends;
using GGone.API.Models.Progress;
using GGone.API.Models.Tasks;
using GGone.API.Models.AI;
using GGone.API.Models.Badges;
using Microsoft.EntityFrameworkCore;

namespace GGone.API.Data
{
    public class GGoneDbContext : DbContext
    {
        public GGoneDbContext(DbContextOptions<GGoneDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Addiction> Addictions { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<DailyTaskLog> DailyTaskLogs { get; set; }
        public DbSet<UserHealthRecord> UserHealthRecords { get; set; }
        public DbSet<DietDay> DietDays { get; set; }
        public DbSet<WeeklyDietPlan> WeeklyDietPlans { get; set; }
        public DbSet<ChatHistory> ChatHistories { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<Friendship> Friendships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GGoneDbContext).Assembly);

            // Arkadaşlık tablosu için döngüsel silme engelleme yapılandırması
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Cascade yerine Restrict kullanıyoruz

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Friend)
                .WithMany()
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.Restrict); // Cascade yerine Restrict kullanıyoruz
        }

    }
}