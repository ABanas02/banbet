using banbet.Data;
using banbet.Models;
using banbet.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace banbet.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/{controller}")]
    public class OddsController: ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<OddsController> _logger;

        public OddsController(ApplicationDbContext dbContext, ILogger<OddsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetOdds()
        {
            var odds = await _dbContext.Odds
                .Include(o => o.Event)
                .ThenInclude(e => e.Bets)
                .AsNoTracking()
                .ToListAsync();
            
            return Ok(odds);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOdd([FromRoute] int id)
        {
            var odd = await _dbContext.Odds
                .Include(o => o.Event)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OddsID == id);

            if (odd is null)
            {
                return BadRequest($"Nie znaleziono odds o id:{id}");
            }

            return Ok(odd);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveOdd([FromRoute] int id)
        {
            var odd = await _dbContext.Odds.FindAsync(id);
            
            if (odd is null)
            {
                return BadRequest($"Nie znaleziono odds o id:{id}");
            }

            _dbContext.Odds.Remove(odd);

            await _dbContext.SaveChangesAsync();

            return Ok($"Usunieto odds o id:{id}");
        }

        [HttpPost("SetOdds")]
        public async Task<IActionResult> SetOdds([FromBody] OddDto oddDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var eventExists = await _dbContext.Events.AnyAsync(e => e.EventID == oddDto.EventID);
            if (!eventExists)
                return NotFound($"Wydarzenie o ID {oddDto.EventID} nie istnieje.");

            if (oddDto.TeamID.HasValue)
            {
                var team = await _dbContext.Teams.FindAsync(oddDto.TeamID.Value);
                if (team == null)
                    return NotFound($"Drużyna o ID {oddDto.TeamID.Value} nie istnieje.");
            }

            var newOdd = new Odd
            {
                EventID = oddDto.EventID,
                BetType = oddDto.BetType,
                OddsValue = oddDto.OddsValue,
                LastUpdated = DateTime.UtcNow,
                TeamID = oddDto.TeamID,
                TeamName = oddDto.TeamName
            };

            _dbContext.Odds.Add(newOdd);
            await _dbContext.SaveChangesAsync();

            return Ok(newOdd);
        }
    }

}