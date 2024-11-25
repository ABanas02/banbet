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
using System.Text.Json;

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

        
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _dbContext.Users.ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] int? id)
        {
            if (id is null) {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Nieprawidłowy token użytkownika.");
                }
                id = userId;
            }
            var user = await _dbContext.Users.FindAsync(id);
            if (user is null)
            {
                return NotFound($"Uzytkownik o id: {id} nie istnieje");
            }
            return Ok(user);
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

            string role = "User";
            if (!string.IsNullOrEmpty(registerDto.Role) && registerDto.Role == "Admin")
            {
                role = "Admin";
            }

            

            var user = new User 
            {
                Username = registerDto.Username,
                PasswordHash = passwordHash,
                Email = registerDto.Email,
                Role = role
            };

            _dbContext.Users.Add(user);

            await _dbContext.SaveChangesAsync();

            return Ok($"Utworzono nowego użytkownika:{user.Username} {user.BirthDate} {user.Email}");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if (user is null)
            {
                return NotFound($"Uzytkownik o id: {id} nie istnieje");
            }
            
            _dbContext.Users.Remove(user);

            await _dbContext.SaveChangesAsync();

            return Ok($"Usunięto uzytkownika o nazwie: {user.Username}, id: {user.UserID}");
        }

        
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user is null)
            {
                return NotFound("Nie znaleziono uzytkownika");
            }

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
                new Claim("role", user.Role)
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