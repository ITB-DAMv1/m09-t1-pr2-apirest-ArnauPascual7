using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Vote> Votes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Game>()
                .HasKey(g => g.Id);
            modelBuilder.Entity<AppUser>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<Vote>()
                .HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(v => v.User);
            modelBuilder.Entity<Vote>()
                .HasOne<Game>()
                .WithMany()
                .HasForeignKey(v => v.Game);
        }
    }
}
