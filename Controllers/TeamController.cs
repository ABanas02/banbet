using banbet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using banbet.Models.DTOs;
using banbet.Models;

namespace banbet.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/{controller}")]
    public class TeamController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<TeamController> _logger;

        public TeamController(ApplicationDbContext dbContext, ILogger<TeamController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            var teams = await _dbContext.Teams.ToListAsync();

            if (teams.Count == 0 || teams is null)
            {
                return NotFound("Nie znaleziono zadnych druzyn");
            }

            return Ok(teams);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeam([FromRoute] int id)
        {
            var team = await _dbContext.Teams.FindAsync(id);

            if (team is null)
            {
                return NotFound($"Nie znaleziono druzyny o id: {id}");
            }

            return Ok(team);
        }

        [HttpPost("AddTeam")]
        public async Task<IActionResult> AddTeam([FromBody] TeamDto teamDto)
        {
            var team = new Team
            {
                TeamName = teamDto.TeamName
            };

            _dbContext.Teams.Add(team);

            await _dbContext.SaveChangesAsync();

            return Ok(team);
        }

        [HttpGet("GetTeamsFromEvent/{eventID}")]
        public async Task<IActionResult> GetTeamsFromEvent([FromRoute] int eventID) 
        {
            var teams = await _dbContext.EventTeams
                                        .Where(et => et.EventID == eventID)
                                        .Select(et => new {
                                            et.Team.TeamID,
                                            et.Team.TeamName
                                        })
                                        .AsNoTracking()
                                        .ToListAsync();
            
            return Ok(teams);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam([FromRoute] int id)
        {
            var team = _dbContext.Teams.Find(id);

            if (team is null)
            {
                return NotFound($"Nie znaleziono druzyny z id:{id}");
            }

            _dbContext.Teams.Remove(team);

            await _dbContext.SaveChangesAsync();

            return Ok($"Usunięto druzyne {team.TeamName}");
        }

        [HttpPost("AddTeamsToEvent")]
        public async Task<IActionResult> AddTeamsToEvent([FromBody] AddTeamsToEventDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var eventItem = await _dbContext.Events.FindAsync(dto.EventID);
            if (eventItem == null)
                return NotFound($"Wydarzenie o ID {dto.EventID} nie istnieje.");

            foreach (var teamId in dto.TeamIDs)
            {
                var team = await _dbContext.Teams.FindAsync(teamId);
                if (team == null)
                    return NotFound($"Drużyna o ID {teamId} nie istnieje.");

                var eventTeam = new EventTeam
                {
                    EventID = dto.EventID,
                    TeamID = teamId
                };

                _dbContext.EventTeams.Add(eventTeam);
            }

            await _dbContext.SaveChangesAsync();

            return Ok($"Drużyny zostały dodane do wydarzenia o ID {dto.EventID}.");
        }
    }
}