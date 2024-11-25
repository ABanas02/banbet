using banbet.Services;
using banbet.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using banbet.CustomExceptions;

namespace banbet.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly TeamService _teamService;
        public TeamController(TeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            try
            {
                var teams = await _teamService.GetTeams();
                return Ok(teams);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Błąd serwera.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeam([FromRoute] int id)
        {
            try
            {
                var team = await _teamService.GetTeam(id);
                return Ok(team);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Błąd serwera.");
            }
        }

        [HttpPost("AddTeam")]
        public async Task<IActionResult> AddTeam([FromBody] TeamDto teamDto)
        {
            try
            {
                var team = await _teamService.AddTeam(teamDto);
                return Ok(team);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Błąd serwera.");
            }
        }

        [HttpGet("GetTeamsFromEvent/{eventID}")]
        public async Task<IActionResult> GetTeamsFromEvent([FromRoute] int eventID)
        {
            try
            {
                var teams = await _teamService.GetTeamsFromEvent(eventID);
                return Ok(teams);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Błąd serwera.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam([FromRoute] int id)
        {
            try
            {
                await _teamService.DeleteTeam(id);
                return Ok(new { Message = $"Usunięto drużynę o ID {id}." });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Błąd serwera.");
            }
        }

        [HttpPost("AddTeamsToEvent")]
        public async Task<IActionResult> AddTeamsToEvent([FromBody] AddTeamsToEventDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var resultMessage = await _teamService.AddTeamsToEvent(dto);
                return Ok(new { Message = resultMessage });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Błąd serwera.");
            }
        }
    }
}
