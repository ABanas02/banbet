using banbet;
using banbet.Data;
using banbet.Models;
using banbet.Models.DTOs;
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

        public async Task<List<EventResponseDto>> GetRecommendedEvents(int? userId) 
        {
            if (!userId.HasValue)
            {
                return await GetDefaultEvents();
            }

            var userBets = await _dbContext.Bets
                .Where(b => b.UserID == userId)
                .Include(b => b.Event)
                .ToListAsync();

            if (!userBets.Any())
            {
                return await GetDefaultEvents();
            }

            var preferredCategories = userBets
                .GroupBy(b => b.Event.Category)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .ToList();

            var upcomingEvents = await _dbContext.Events
                .Include(e => e.Odds)
                .Include(e => e.EventTeams)
                    .ThenInclude(et => et.Team)
                .Where(e => e.EventStatus == EventStatus.Upcoming)
                .ToListAsync();

            var recommendedEvents = upcomingEvents
                .Select(e => new
                {
                    Event = e,
                    Score = CalculateEventScore(e, preferredCategories)
                })
                .OrderByDescending(e => e.Score)
                .Select(e => new EventResponseDto
                {
                    EventID = e.Event.EventID,
                    EventName = e.Event.EventName,
                    StartDateTime = e.Event.StartDateTime,
                    EventStatus = e.Event.EventStatus,
                    Result = e.Event.Result,
                    Description = e.Event.Description,
                    Category = e.Event.Category,
                    Teams = e.Event.EventTeams.Select(et => new TeamDto
                    {
                        TeamID = et.Team.TeamID,
                        TeamName = et.Team.TeamName
                    }).ToList(),
                    Odds = e.Event.Odds.Select(o => new OddDto
                    {
                        BetType = o.BetType,
                        OddsValue = o.OddsValue,
                        TeamID = o.TeamID,
                        TeamName = o.TeamName
                    }).ToList()
                })
                .ToList();

            return recommendedEvents;
        }

        private async Task<List<EventResponseDto>> GetDefaultEvents()
        {
            var events = await _dbContext.Events
                .Include(e => e.Odds)
                .Include(e => e.EventTeams)
                    .ThenInclude(et => et.Team)
                .Where(e => e.EventStatus == EventStatus.Upcoming)
                .OrderBy(e => e.StartDateTime)
                .Take(10)
                .ToListAsync();

            return events.Select(e => new EventResponseDto
            {
                EventID = e.EventID,
                EventName = e.EventName,
                StartDateTime = e.StartDateTime,
                EventStatus = e.EventStatus,
                Result = e.Result,
                Description = e.Description,
                Category = e.Category,
                Teams = e.EventTeams.Select(et => new TeamDto
                {
                    TeamID = et.Team.TeamID,
                    TeamName = et.Team.TeamName
                }).ToList(),
                Odds = e.Odds.Select(o => new OddDto
                {
                    BetType = o.BetType,
                    OddsValue = o.OddsValue,
                    TeamID = o.TeamID,
                    TeamName = o.TeamName
                }).ToList()
            }).ToList();
        }

        private double CalculateEventScore(Event evt, List<Category> preferredCategories)
        {
            int categoryIndex = preferredCategories.IndexOf(evt.Category);
            if (categoryIndex != -1)
            {
                return (preferredCategories.Count - categoryIndex) * 2;
            }
            return 0;
        }
    }
}
