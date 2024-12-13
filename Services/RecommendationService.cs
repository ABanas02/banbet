using banbet;
using banbet.Data;
using banbet.Models;
using banbet.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace banbet.Services 
{
    public enum RecommendationStrategy
    {
        Basic,
        AI
    }

    public class RecommendationService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AIRecommendationService _aiRecommendationService;

        public RecommendationService(
            ApplicationDbContext dbContext,
            AIRecommendationService aiRecommendationService
        ) 
        {
            _dbContext = dbContext;
            _aiRecommendationService = aiRecommendationService;
        }

        public async Task<List<EventResponseDto>> GetRecommendedEvents(int? userId, RecommendationStrategy strategy = RecommendationStrategy.Basic) 
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

            var upcomingEvents = await _dbContext.Events
                .Include(e => e.Odds)
                .Include(e => e.EventTeams)
                    .ThenInclude(et => et.Team)
                .Where(e => e.EventStatus == EventStatus.Upcoming)
                .ToListAsync();

            if (strategy == RecommendationStrategy.AI)
            {
                return await GetAIRecommendedEvents(userBets, upcomingEvents);
            }

            return await GetBasicRecommendedEvents(userBets, upcomingEvents);
        }

        private async Task<List<EventResponseDto>> GetAIRecommendedEvents(List<Bet> userBets, List<Event> upcomingEvents)
        {
            var recommendedCategories = await _aiRecommendationService.GetAIRecommendedEventIds(userBets);
            
            var categoryScores = recommendedCategories
                .Select((category, index) => new { Category = category, Score = recommendedCategories.Count - index })
                .ToDictionary(x => x.Category, x => x.Score);

            var recommendedEvents = upcomingEvents
                .Select(e => new
                {
                    Event = e,
                    Score = categoryScores.GetValueOrDefault((int)e.Category, 0)
                })
                .OrderByDescending(e => e.Score)
                .Select(e => MapToEventResponseDto(e.Event))
                .ToList();

            return recommendedEvents;
        }

        private async Task<List<EventResponseDto>> GetBasicRecommendedEvents(List<Bet> userBets, List<Event> upcomingEvents)
        {
            var preferredCategories = userBets
                .GroupBy(b => b.Event.Category)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .ToList();

            var recommendedEvents = upcomingEvents
                .Select(e => new
                {
                    Event = e,
                    Score = CalculateEventScore(e, preferredCategories)
                })
                .OrderByDescending(e => e.Score)
                .Select(e => MapToEventResponseDto(e.Event))
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

            return events.Select(e => MapToEventResponseDto(e)).ToList();
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

        private EventResponseDto MapToEventResponseDto(Event evt)
        {
            return new EventResponseDto
            {
                EventID = evt.EventID,
                EventName = evt.EventName,
                StartDateTime = evt.StartDateTime,
                EventStatus = evt.EventStatus,
                Result = evt.Result,
                Description = evt.Description,
                Category = evt.Category,
                Teams = evt.EventTeams.Select(et => new TeamDto
                {
                    TeamID = et.Team.TeamID,
                    TeamName = et.Team.TeamName
                }).ToList(),
                Odds = evt.Odds.Select(o => new OddDto
                {
                    BetType = o.BetType,
                    OddsValue = o.OddsValue,
                    TeamID = o.TeamID,
                    TeamName = o.TeamName
                }).ToList()
            };
        }
    }
}
