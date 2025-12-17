using GGone.API.Models.Addiction;
using GGone.API.Models.Auth;
using GGone.API.Models.Exercises;
using GGone.API.Models.Tasks;
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
    }
}