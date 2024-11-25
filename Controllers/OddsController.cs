using banbet.CustomExceptions;
using banbet.Data;
using banbet.Models;
using banbet.Models.DTOs;
using banbet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace banbet.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/{controller}")]
    public class OddsController: ControllerBase
    {
        private readonly OddsService _oddsService;

        public OddsController(ApplicationDbContext dbContext, ILogger<OddsController> logger, OddsService oddsService)
        {
            _oddsService = oddsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOdds()
        {
            var odds = await _oddsService.GetOdds();
            
            return Ok(odds);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOdd([FromRoute] int id)
        {
            try 
            {
                var odd = await _oddsService.GetOdd(id);
                return Ok(odd);
            } 
            catch (EntityNotFoundException ex) 
            {
                return NotFound(new { Message = ex.Message});
            } 
            catch (Exception) 
            {
                return StatusCode(500, "Błąd serwera");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveOdd([FromRoute] int id)
        {
            try 
            {
                await _oddsService.DeleteOdd(id);
                return Ok($"Usunięto odds o id:{id}");
            } 
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message});
            }
            catch (Exception)
            {
                return StatusCode(500, "Błąd serwera");
            }
        }

        [HttpPost("SetOdds")]
        public async Task<IActionResult> SetOdds([FromBody] OddDto oddDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newOdd = await _oddsService.SetOdds(oddDto);
                return Ok(newOdd);
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