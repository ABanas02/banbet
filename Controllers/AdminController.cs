using banbet.Data;
using banbet.Models;
using banbet.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace banbet.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext dbContext, ILogger<AdminController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        

        
    
    }
}