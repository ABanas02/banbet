using banbet;
using banbet.Data;
using banbet.Models;
using Microsoft.EntityFrameworkCore;

namespace banbet.Services 
{
    public class RecommendationService
    {
        private readonly ApplicationDbContext _dbContext;

        public RecommendationService(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        // public async Task<List<Event>> GetRecommendedEvents(int userId) 
        // {
        //     var userBets = await _dbContext.Bets
        //         .Where(b => b.UserID == userId)
        //         .Include(b => b.Event)
        //             .ThenInclude(e => e.Category)
        //         .ToListAsync();

        //      if (!userBets.Any())
        //     {
        //         //return await GetDefaultEvents();
        //     }

        //     var preferredCategories = userBets
        //         .GroupBy(b => b.Event.Category)
        //         .OrderByDescending(g => g.Count())
        //         .Select(g => g.Key)
        //         .ToList();
        // }

    }
}