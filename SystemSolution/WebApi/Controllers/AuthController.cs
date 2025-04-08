using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.DTOs;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<AppUser> userManager, ILogger<AuthController> logger, IConfiguration configuration)
        {
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("registre")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO reg)
        {
            AppUser user = new AppUser
            {
                Name = reg.Name,
                UserName = reg.Username,
                Email = reg.Email
            };
            IdentityResult result = await _userManager.CreateAsync(user, reg.Password);

            if (result.Succeeded) return Ok("Usuari registrat correctament");

            return BadRequest(result.Errors);
        }
        [HttpPost("admin/registre")]
        public async Task<IActionResult> AdminRegister([FromBody] RegisterDTO reg)
        {
            AppUser user = new AppUser
            {
                Name = reg.Name,
                UserName = reg.Username,
                Email = reg.Email
            };
            IdentityResult result = await _userManager.CreateAsync(user, reg.Password);
            IdentityResult resultRole = new IdentityResult();

            if (result.Succeeded)
            {
                resultRole = await _userManager.AddToRoleAsync(user, "Admin");
            }

            if (result.Succeeded && resultRole.Succeeded)
            {
                return Ok("Usuari ADMIN registrat correctament");
            }

            return BadRequest(result.Errors);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            AppUser? user = await _userManager.FindByEmailAsync(login.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, login.Password))
            {
                return Unauthorized("Email o contrasenya incorrectes");
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            return Ok();
        }
    }
}
