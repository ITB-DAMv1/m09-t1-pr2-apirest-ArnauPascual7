using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GamesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames()
        {
            Debug.WriteLine("?: User Name Logged -> " + User.Identity?.Name);
            Debug.WriteLine("?: User Id -> " + User.FindFirstValue(ClaimTypes.NameIdentifier));
            Debug.WriteLine("?: User Role -> " + User.FindFirstValue(ClaimTypes.Role));

            return await _context.Games.ToListAsync();
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame(int id)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            return game;
        }

        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameDTO game)
        {
            var dbGame = await _context.Games.FindAsync(id);
            if (dbGame == null)
            {
                return NotFound();
            }

            dbGame.Title = game.Title;
            dbGame.Description = game.Description;
            dbGame.DevTeam = game.DevTeam;

            _context.Update(dbGame);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(dbGame);
        }

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(GameDTO game)
        {
            var dbGame = new Game()
            {
                Title = game.Title,
                Description = game.Description,
                DevTeam = game.DevTeam
            };

            _context.Games.Add(dbGame);
            await _context.SaveChangesAsync();

            return Ok(game);
                // CreatedAtAction("GetGame", new { id = await _context.Games.FindAsync() }, game);
        }

        // DELETE: api/Games/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Games/5
        [Authorize(Roles = "Admin, User")]
        [HttpPost("{id}")]
        public async Task<IActionResult> Vote(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound("El joc no s'ha trobat");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return Unauthorized("No ets un usuari autoritzat");
            }

            var vote = new Vote()
            {
                GameId = game.Id,
                UserId = user.Id,
                Game = game,
                User = user
            };

            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGame", new { id = game.Id }, game);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("Votes")]
        public async Task<ActionResult<IEnumerable<Vote>>> GetVotes()
        {
            return await _context.Votes.ToListAsync();
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    }
}
