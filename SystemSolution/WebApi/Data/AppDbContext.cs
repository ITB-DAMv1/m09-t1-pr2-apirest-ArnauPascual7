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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Game>()
                .HasMany(g => g.Votes)
                .WithOne(v => v.Game)
                .HasForeignKey(v => v.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AppUser>()
                .HasMany(u => u.Votes)
                .WithOne(v => v.User)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            /*modelBuilder.Entity<Vote>()
                .HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(v => v.User);
            modelBuilder.Entity<Vote>()
                .HasOne<Game>()
                .WithMany()
                .HasForeignKey(v => v.Game);*/
        }
    }
}
