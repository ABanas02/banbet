using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using banbet.Data;
using banbet.Models;
using banbet.Models.DTOs;
using banbet.CustomExceptions;
using System.Collections.Generic;
using System.Linq;

namespace banbet.Services
{
    public class BetsService
    {
        private readonly ApplicationDbContext _dbContext;

        public BetsService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Bet> PlaceBet(BetDto betDto, ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Nieprawidłowy token użytkownika.");
            }

            var userEntity = await _dbContext.Users.FindAsync(userId);
            if (userEntity == null)
                throw new EntityNotFoundException("Użytkownik nie został znaleziony.");

            if (userEntity.VirtualBalance < betDto.BetAmount)
                throw new InvalidOperationException("Niewystarczające saldo.");

            var odd = await _dbContext.Odds
                .Include(o => o.Event)
                .FirstOrDefaultAsync(o => o.OddsID == betDto.OddsID);

            if (odd == null)
                throw new InvalidOperationException("Nieprawidłowy kurs.");

            if (odd.Event.EventStatus != EventStatus.Upcoming)
                throw new InvalidOperationException("Wydarzenie nie jest dostępne do obstawiania.");

            userEntity.VirtualBalance -= betDto.BetAmount;

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

            return newBet;
        }

        public async Task<List<Bet>> GetUserBets(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Nieprawidłowy token użytkownika.");
            }

            var bets = await _dbContext.Bets
                .Include(b => b.Odd)
                    .ThenInclude(o => o.Event)
                    .ThenInclude(o => o.EventTeams)
                    .ThenInclude(et => et.Team)
                .Where(b => b.UserID == userId)
                .AsNoTracking()
                .ToListAsync();

            return bets;
        }

        public async Task<string> ResolveMatchWinnerBets(ResolveMatchWinnerDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Nieprawidłowe dane wejściowe.");

            var eventItem = await _dbContext.Events.SingleOrDefaultAsync(e => e.EventID == dto.EventID);
            if (eventItem == null)
                throw new EntityNotFoundException("Wydarzenie nie zostało znalezione.");

            var teamExists = await _dbContext.Teams.AnyAsync(t => t.TeamID == dto.TeamID);
            if (!teamExists)
                throw new EntityNotFoundException("Drużyna nie została znaleziona.");

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

                    return "Zakłady rozliczone!";
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<Bet> GetBet(int betId, ClaimsPrincipal user)
        {
            var bet = await _dbContext.Bets
                .Include(b => b.Odd)
                    .ThenInclude(o => o.Event)
                .FirstOrDefaultAsync(b => b.BetID == betId);

            if (bet == null)
                throw new EntityNotFoundException("Zakład nie został znaleziony.");

            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId) || bet.UserID != userId)
            {
                throw new UnauthorizedAccessException("Brak dostępu do tego zakładu.");
            }

            return bet;
        }
    }
}
