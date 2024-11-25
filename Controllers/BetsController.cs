using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using banbet.Services;
using banbet.Models.DTOs;
using banbet.CustomExceptions;
using Microsoft.Extensions.Logging;

namespace banbet.Controllers
{
    //[Authorize(Roles = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class BetsController : ControllerBase
    {
        private readonly BetsService _betsService;

        public BetsController(BetsService betsService)
        {
            _betsService = betsService;
        }

        [HttpPost("PlaceBet")]
        public async Task<IActionResult> PlaceBet([FromBody] BetDto betDto)
        {
            try
            {
                var newBet = await _betsService.PlaceBet(betDto, User);
                return Ok(newBet);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wystąpił błąd podczas składania zakładu.");
            }
        }

        [HttpGet("MyBets")]
        public async Task<IActionResult> GetUserBets()
        {
            try
            {
                var bets = await _betsService.GetUserBets(User);
                return Ok(bets);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wystąpił błąd podczas pobierania zakładów użytkownika.");
            }
        }

        [HttpPost("ResolveMatchWinner")]
        public async Task<IActionResult> ResolveMatchWinnerBets([FromBody] ResolveMatchWinnerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var resultMessage = await _betsService.ResolveMatchWinnerBets(dto);
                return Ok(new { Message = resultMessage });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wystąpił błąd podczas rozliczania zakładów.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBet(int id)
        {
            try
            {
                var bet = await _betsService.GetBet(id, User);
                return Ok(bet);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wystąpił błąd podczas pobierania zakładu.");
            }
        }
    }
}
