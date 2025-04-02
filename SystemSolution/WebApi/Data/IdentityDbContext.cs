using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public class IdentityDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<AppUser> Users { get; set; }

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }
    }
}
