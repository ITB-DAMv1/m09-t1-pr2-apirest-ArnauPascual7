﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
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
            var user = new AppUser
            {
                Name = reg.Name,
                UserName = reg.Username,
                Email = reg.Email
            };
            var result = await _userManager.CreateAsync(user, reg.Password);
            var resultRole = new IdentityResult();

            if (result.Succeeded)
            {
                resultRole = await _userManager.AddToRoleAsync(user, "User");
            }

            if (result.Succeeded && resultRole.Succeeded)
            {
                return Ok("Usuari registrat correctament");
            }

            return BadRequest(result.Errors.Select(e => e.Description));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/registre")]
        public async Task<IActionResult> AdminRegister([FromBody] RegisterDTO reg)
        {
            var user = new AppUser
            {
                Name = reg.Name,
                UserName = reg.Username,
                Email = reg.Email
            };
            var result = await _userManager.CreateAsync(user, reg.Password);
            var resultRole = new IdentityResult();

            if (result.Succeeded)
            {
                resultRole = await _userManager.AddToRoleAsync(user, "Admin");
            }

            if (result.Succeeded && resultRole.Succeeded)
            {
                return Ok("Usuari ADMIN registrat correctament");
            }

            return BadRequest(result.Errors.Select(e => e.Description));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(login.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, login.Password))
            {
                return Unauthorized("Email o contrasenya incorrectes");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name ?? user.UserName ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
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
