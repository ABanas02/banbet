using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using banbet.Data;
using banbet.Models;
using banbet.Models.DTOs;
using banbet.CustomExceptions;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace banbet.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AccountService(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<List<User>> GetUsers()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User> GetUser(int? id, ClaimsPrincipal user)
        {
            if (id is null)
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    throw new UnauthorizedAccessException("Nieprawidłowy token użytkownika.");
                }
                id = userId;
            }

            var userEntity = await _dbContext.Users.FindAsync(id);
            if (userEntity is null)
            {
                throw new EntityNotFoundException($"Użytkownik o ID {id} nie istnieje.");
            }
            return userEntity;
        }

        public async Task<string> Register(RegisterDto registerDto)
        {
            if (registerDto == null)
            {
                throw new ArgumentNullException(nameof(registerDto), "Nieprawidłowe dane użytkownika.");
            }

            if (string.IsNullOrEmpty(registerDto.Username) || string.IsNullOrEmpty(registerDto.Password) || string.IsNullOrEmpty(registerDto.Email))
            {
                throw new ArgumentException("Wszystkie pola są wymagane.");
            }

            if (await _dbContext.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                throw new InvalidOperationException("Użytkownik o danej nazwie już istnieje!");
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

            return $"Utworzono nowego użytkownika: {user.Username}";
        }

        public async Task<string> DeleteUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if (user is null)
            {
                throw new EntityNotFoundException($"Użytkownik o ID {id} nie istnieje.");
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return $"Usunięto użytkownika: {user.Username}, ID: {user.UserID}";
        }

        public async Task<string> Login(LoginDto loginDto)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user is null)
            {
                throw new EntityNotFoundException("Nie znaleziono użytkownika.");
            }

            if (BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                var token = GenerateJwtToken(user);
                return token;
            }

            throw new InvalidOperationException("Weryfikacja nieudana.");
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
