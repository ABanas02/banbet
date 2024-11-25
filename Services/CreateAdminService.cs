using banbet.Data;
using banbet.Models;
using Microsoft.EntityFrameworkCore;

namespace banbet.Services
{
    public class CreateAdminService 
    {
        private readonly ApplicationDbContext _dbContext;
        public CreateAdminService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAdmin() 
        {
            var doesExist = await _dbContext.Users.AnyAsync(u => u.Username == "admin");

            if (!doesExist) {
                var admin = new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                    Email = "admin@admin",
                    Role = "Admin"
                };

                _dbContext.Users.Add(admin);

                await _dbContext.SaveChangesAsync();
            }
        }

    }
}