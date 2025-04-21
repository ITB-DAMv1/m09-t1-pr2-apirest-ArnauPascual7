using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApi.DTOs;
using WebApi.Models;
namespace WebApi.Tools
{
    public static class ModelBuilderHelper
    {
        public static void Seed(this ModelBuilder builder)
        {
            List<GameDTO> games = new List<GameDTO>()
            {
                new GameDTO
                {
                    Id = 1,
                    Title = "Blasphemous",
                    Description = "A dark and brutal Metroidvania set in a world inspired by Spanish culture and religion.",
                    DevTeam = "The Game Kitchen"
                },
                new GameDTO
                {
                    Id = 2,
                    Title = "Darkest Dungeon",
                    Description = "A challenging turn-based RPG that focuses on the psychological stresses of adventuring.",
                    DevTeam = "Red Hook Studios"
                },
                new GameDTO
                {
                    Id = 3,
                    Title = "THE FINALS",
                    Description = "A fast-paced, team-based shooter with a unique destructible environment.",
                    DevTeam = "Embark Studios"
                },
                new GameDTO
                {
                    Id = 4,
                    Title = "Five Nights at Freddy's: Security Breach",
                    Description = "A survival horror game set in a large shopping mall filled with animatronic characters.",
                    DevTeam = "Steel Wool Studios"
                },
                new GameDTO
                {
                    Id = 5,
                    Title = "GRIS",
                    Description = "A visually stunning platformer that tells a story of loss and recovery through beautiful art and music.",
                    DevTeam = "Nomada Studio"
                }
            };
            builder.Entity<Game>().HasData(games);
        }
        public static async Task SeedUsersAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<AppUser>>();

            AppUser admin = new AppUser
            {
                Name = "Admin",
                UserName = "admin",
                Email = "admin@example.com"
            };
            AppUser user = new AppUser
            {
                Name = "User",
                UserName = "user",
                Email = "user@example.com"
            };
            var password = "P@ssw0rd123";

            var result = await userManager.CreateAsync(admin, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "User");
            }
        }
    }
}
