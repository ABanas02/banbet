using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using banbet.Data;
using banbet.Models;
using banbet.Models.DTOs;

namespace banbet.Controllers
{
    //[Authorize(Roles = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class BetsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public BetsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("PlaceBet")]
        public async Task<IActionResult> PlaceBet([FromBody] BetDto betDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Nieprawidłowy token użytkownika.");
            }

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Użytkownik nie został znaleziony.");

            // if (!user.IsIdentityVerified)
            //     return BadRequest("Wiek użytkownika nie zweryfikowany!");
            
            if (user.VirtualBalance < betDto.BetAmount)
                return BadRequest("Niewystarczające saldo.");

            var odd = await _dbContext.Odds
                .Include(o => o.Event)
                .FirstOrDefaultAsync(o => o.OddsID == betDto.OddsID);

            if (odd == null)
                return BadRequest("Nieprawidłowy kurs.");
            if (odd.Event.EventStatus != EventStatus.Upcoming)
                return BadRequest("Wydarzenie nie jest dostępne do obstawiania.");

            user.VirtualBalance -= betDto.BetAmount;

            var potentialPayout = betDto.BetAmount * odd.OddsValue;

            var newBet = new Bet
            {
                UserID = userId,
                EventID = odd.EventID,
                OddsID = betDto.OddsID,
                BetAmount = betDto.BetAmount,
                BetStatus = BetStatus.Open,
                BetDate = DateTime.UtcNow
            };

            _dbContext.Bets.Add(newBet);

            await _dbContext.SaveChangesAsync();

            return Ok(newBet);
        }


        [HttpGet("MyBets")]
        public async Task<IActionResult> GetUserBets()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Nieprawidłowy token użytkownika.");
            }

            var bets = await _dbContext.Bets
                .Include(b => b.Odd)
                .ThenInclude(o => o.Event)
                .Where(b => b.UserID == userId)
                .ToListAsync();

            return Ok(bets);
        }

        [HttpPost("ResolveMatchWinner")]
        public async Task<IActionResult> ResolveMatchWinnerBets([FromBody] ResolveMatchWinnerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (dto == null)
                return BadRequest("Nieprawidłowe dane wejściowe.");

            var eventItem = await _dbContext.Events.SingleOrDefaultAsync(e => e.EventID == dto.EventID);
            if (eventItem == null)
                return NotFound("Wydarzenie nie zostało znalezione.");

            var teamExists = await _dbContext.Teams.AnyAsync(t => t.TeamID == dto.TeamID);
            if (!teamExists)
                return NotFound("Drużyna nie została znaleziona.");

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    eventItem.EventStatus = EventStatus.Finished;
                    eventItem.Result = dto.TeamID.ToString();

                    var bets = await _dbContext.Bets
                        .Include(b => b.Odd)
                        .Include(b => b.User)
                        .Where(b => b.EventID == dto.EventID && b.BetStatus == BetStatus.Open)
                        .ToListAsync();

                    foreach (var bet in bets)
                    {
                        if (bet.Odd.BetType == BetType.MatchWinner)
                        {
                            if (bet.Odd.TeamID == dto.TeamID)
                            {
                                bet.BetStatus = BetStatus.Won;
                                decimal payout = bet.BetAmount * bet.Odd.OddsValue;
                                bet.User.VirtualBalance += payout;
                            }
                            else
                            {
                                bet.BetStatus = BetStatus.Lost;
                            }
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok("Zakłady rozliczone!");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Wystąpił błąd podczas rozliczania zakładów.");
                }
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetBet(int id)
        {
            var bet = await _dbContext.Bets
                .Include(b => b.Odd)
                .ThenInclude(o => o.Event)
                .FirstOrDefaultAsync(b => b.BetID == id);

            if (bet == null)
                return NotFound("Zakład nie został znaleziony.");
                
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId) || bet.UserID != userId)
            {
                return Unauthorized("Brak dostępu do tego zakładu.");
            }

            return Ok(bet);
        }
    }
}
