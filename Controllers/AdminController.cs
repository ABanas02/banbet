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

        [HttpPost("CreateEvent")]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto eventDto)
        {
            var newEvent = new Event
            {
                EventName = eventDto.EventName,
                StartDateTime = eventDto.StartDateTime,
                Description = eventDto.Description,
                EventStatus = EventStatus.Upcoming
            };

            _dbContext.Events.Add(newEvent);
            await _dbContext.SaveChangesAsync();

            return Ok(newEvent);
        }

        [HttpGet("Events")]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _dbContext.Events
                .Include(e => e.Odds)
                .Where(e => e.EventStatus == EventStatus.Upcoming)
                .ToListAsync();

            return Ok(events);
        }

        [HttpGet("Odds")]
        public async Task<IActionResult> GetOdds()
        {
            var odds = await _dbContext.Odds
                .Include(o => o.Event)
                .ToListAsync();
            
            return Ok(odds);
        }

        [HttpPost("SetOdds")]
        public async Task<IActionResult> SetOdds([FromBody] OddDto oddDto)
        {
            var eventExists = await _dbContext.Events.AnyAsync(e => e.EventID == oddDto.EventID);
            if (!eventExists)
                return NotFound($"Wydarzenie o ID {oddDto.EventID} nie istnieje.");

            var newOdd = new Odd
            {
                EventID = oddDto.EventID,
                BetType = oddDto.BetType,
                OddsValue = oddDto.OddsValue,
                LastUpdated = DateTime.UtcNow
            };

            _dbContext.Odds.Add(newOdd);
            
            await _dbContext.SaveChangesAsync();

            return Ok(newOdd);
        }
    
    }
}