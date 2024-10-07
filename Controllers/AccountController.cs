using banbet.Data;
using banbet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using banbet.Models.DTOs;

namespace banbet.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        
        public AccountController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (registerDto == null) {
                return BadRequest("Nieprawidłowe dane użytkownika");
            }

            if (string.IsNullOrEmpty(registerDto.Username) || string.IsNullOrEmpty(registerDto.Password) || string.IsNullOrEmpty(registerDto.Email))
            {
                return BadRequest("Wszystkie pola są wymagane.");
            }
            
            if (await _dbContext.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                return BadRequest("Użytkownik o danej nazwie już istnieje!");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var user = new User 
            {
                Username = registerDto.Username,
                PasswordHash = passwordHash,
                Email = registerDto.Email
            };

            _dbContext.Users.Add(user);

            await _dbContext.SaveChangesAsync();

            return Ok($"Utworzono nowego użytkownika:{user.Username} {user.BirthDate} {user.Email}");
        }



        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _dbContext.Users.SingleAsync(u => u.Username == loginDto.Username);

            

            if (BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
            }


            return BadRequest("Weryfikacja nieudana");
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim("role", "User") // Możesz dodać role, jeśli to konieczne
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}