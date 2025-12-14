using GGone.API.Models.Addiction;
using GGone.API.Models.Auth;
using GGone.API.Models.Exercises;
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
    }
}