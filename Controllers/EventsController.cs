using banbet.Data;
using banbet.Models;
using banbet.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace banbet.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/{controller}")]
    public class EventsController: ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<EventsController> _logger;
        
        public EventsController(ApplicationDbContext dbContext, ILogger<EventsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpPost]
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

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _dbContext.Events
                .Include(e => e.Odds)
                .Where(e => e.EventStatus == EventStatus.Upcoming)
                .ToListAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _dbContext.Events
                .Include(e => e.Odds)
                .Include(e => e.EventTeams)
                    .ThenInclude(et => et.Team)
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
                return NotFound();

            // Mapowanie na DTO
            var eventDto = new EventResponseDto
            {
                EventID = eventItem.EventID,
                EventName = eventItem.EventName,
                StartDateTime = eventItem.StartDateTime,
                EventStatus = eventItem.EventStatus,
                Result = eventItem.Result,
                Description = eventItem.Description,
                Teams = eventItem.EventTeams.Select(et => new TeamDto
                {
                    TeamName = et.Team.TeamName,
                }).ToList(),
                Odds = eventItem.Odds.Select(o => new OddDto
                {
                    BetType = o.BetType,
                    OddsValue = o.OddsValue,
                    TeamID = o.TeamID
                }).ToList()
            };

            return Ok(eventDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent([FromRoute] int id)
        {
            var singleEvent = await _dbContext.Events
                .Include(e => e.Odds)
                .FirstOrDefaultAsync(e => e.EventID == id);
            
            if (singleEvent is null)
            {
                return NotFound($"Event o id:{id} nie istnieje!");
            }

            _dbContext.Events.Remove(singleEvent);
            await _dbContext.SaveChangesAsync();

            return Ok($"Usunieto uzytkownika o id:{id}");
        }
    }
}