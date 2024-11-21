using banbet.Models;
using banbet.Data;
using Microsoft.EntityFrameworkCore;
using banbet.CustomExceptions;
using banbet.Models.DTOs;

namespace banbet.Services
{
    public class OddsService
    {
        private readonly ApplicationDbContext _dbContext;

        public OddsService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Odd>> GetOdds()
        {
            var odds = await _dbContext.Odds
                .Include(o => o.Event)
                .ThenInclude(e => e.Bets)
                .AsNoTracking()
                .ToListAsync();

            return odds;
        }

        public async Task<Odd> GetOdd(int oddId)
        {
            var odd = await _dbContext.Odds
                .Include(o => o.Event)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OddsID == oddId);

            if (odd is null)
            {
                throw new EntityNotFoundException($"Nie znaleziono odds o id:{oddId}");
            }

            return odd;
        }

        public async Task DeleteOdd(int oddId)
        {
            var odd = await _dbContext.Odds.FindAsync(oddId);
            
            if (odd is null)
            {
                throw new EntityNotFoundException($"Nie znaleziono odds o id:{oddId}");
            }

            _dbContext.Odds.Remove(odd);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Odd> SetOdds(OddDto oddDto)
        {
            var eventExists = await _dbContext.Events.AnyAsync(e => e.EventID == oddDto.EventID);
            if (!eventExists)
                throw new EntityNotFoundException($"Wydarzenie o ID {oddDto.EventID} nie istnieje.");

            if (oddDto.TeamID.HasValue)
            {
                var teamExists = await _dbContext.Teams.AnyAsync(t => t.TeamID == oddDto.TeamID.Value);
                if (!teamExists)
                    throw new EntityNotFoundException($"Drużyna o ID {oddDto.TeamID.Value} nie istnieje.");
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

            return newOdd;
        }
    }
}