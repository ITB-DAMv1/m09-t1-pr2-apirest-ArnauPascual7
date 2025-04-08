using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
            IdentityResult resultRole = new IdentityResult();

            if (result.Succeeded)
            {
                resultRole = await _userManager.AddToRoleAsync(user, "User");
            }

            if (result.Succeeded && resultRole.Succeeded)
            {
                return Ok("Usuari registrat correctament");
            }

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

            return Ok(CreateToken(claims.ToArray()));
        }

        private string CreateToken(Claim[] claims)
        {
            var jwtConfig = _configuration.GetSection("JwtSettings");
            var secretKey = jwtConfig["Key"];
            var issuer = jwtConfig["Issuer"];
            var audience = jwtConfig["Audience"];
            var expirationMinutes = int.Parse(jwtConfig["ExpirationMinutes"]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
