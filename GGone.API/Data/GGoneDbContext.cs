using GGone.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GGone.API.Data
{
    public class GGoneDbContext : DbContext
    {
        public GGoneDbContext(DbContextOptions<GGoneDbContext> options) : base(options)
        {
        }

        public DbSet<User > Users { get; set; }
    }
}
