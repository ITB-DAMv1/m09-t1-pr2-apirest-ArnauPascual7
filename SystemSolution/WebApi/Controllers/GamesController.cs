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
        public async Task<ActionResult<IEnumerable<GameWithIdDTO>>> GetGames()
        {
            // This Debugs only works with Swagger
            Debug.WriteLine("?: User Name Logged -> " + User.Identity?.Name);
            Debug.WriteLine("?: User Id -> " + User.FindFirstValue(ClaimTypes.NameIdentifier));
            Debug.WriteLine("?: User Role -> " + User.FindFirstValue(ClaimTypes.Role));

            var dbGames = await _context.Games.ToListAsync();
            var games = dbGames.Select(Tools.GameHelper.SetGameWithIdDTOFromGame).ToList();
            for (var i = 0; i < games.Count; i++)
            {
                var votes = await _context.Votes.Where(v => v.GameId == dbGames[i].Id).ToListAsync();
                games[i].VoteCount = votes.Count;
            }

            return Ok(games);
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDTO>> GetGame(int id)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound("No s'ha trobat el joc");
            }

            var gameDTO = Tools.GameHelper.SetGameDTOFromGame(game);
            gameDTO.VoteCount = await _context.Votes.CountAsync(v => v.GameId == game.Id);

            return gameDTO;
        }

        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameDTO gameDTO)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound("No s'ha trobat el joc");
            }

            game.Title = gameDTO.Title;
            game.Description = gameDTO.Description;
            game.DevTeam = gameDTO.DevTeam;

            _context.Update(game);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
                {
                    return NotFound("No s'ha trobat el joc");
                }
                else
                {
                    throw;
                }
            }

            return Ok(game);
        }

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(GameDTO gameDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var game = Tools.GameHelper.SetGameFromGameDTO(gameDTO);

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGame", new { id = game.Id }, game);
        }

        // DELETE: api/Games/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound("No s'ha trobat el joc");
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Games/5/vote
        [Authorize(Roles = "Admin,User")]
        [HttpPost("{id}/vote")]
        public async Task<IActionResult> Vote(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound("No s'ha trobat el joc");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("No ets un usuari autoritzat");
            }

            var existingVote = await _context.Votes.FirstOrDefaultAsync(v => v.GameId == game.Id && v.UserId == userId);
            if (existingVote != null)
            {
                return BadRequest("Ja has votat aquest joc.");
            }

            var vote = new Vote()
            {
                GameId = game.Id,
                UserId = userId
            };

            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();

            return Ok("Vot registrat correctament");
        }

        // GET: api/Games/votes
        [Authorize(Roles = "Admin,User")]
        [HttpGet("votes")]
        public async Task<IActionResult> GetVotes()
        {
            var votes = await _context.Votes.ToListAsync();
            if (votes == null)
            {
                return NotFound("No hi ha vots");
            }
            return Ok(votes);
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    }
}
