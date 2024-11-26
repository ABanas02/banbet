using banbet.Data;
using banbet.Models.DTOs;
using banbet.Models;
using Microsoft.EntityFrameworkCore;
using banbet.CustomExceptions;

namespace banbet.Services
{
    public class EventsService 
    {
        private readonly ApplicationDbContext _dbContext;

        public EventsService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Event> CreateEvent(EventDto eventDto)
        {
            var startDateTimeUtc = DateTime.SpecifyKind(eventDto.StartDateTime, DateTimeKind.Utc);

            var newEvent = new Event
            {
                EventName = eventDto.EventName,
                StartDateTime = startDateTimeUtc,
                Description = eventDto.Description,
                EventStatus = EventStatus.Upcoming,
                Category = eventDto.Category
            };

            _dbContext.Events.Add(newEvent);
            await _dbContext.SaveChangesAsync();

            return newEvent;
        }

        public async Task<List<EventResponseDto>> GetEvents()
        {
            var events = await _dbContext.Events
                .Include(e => e.Odds)
                .Include(e => e.EventTeams)
                    .ThenInclude(et => et.Team)
                .Where(e => e.EventStatus == EventStatus.Upcoming)
                .AsNoTracking()
                .ToListAsync();            

            var eventDtos = events.Select(eventItem => new EventResponseDto
            {
                EventID = eventItem.EventID,
                EventName = eventItem.EventName,
                StartDateTime = eventItem.StartDateTime,
                EventStatus = eventItem.EventStatus,
                Result = eventItem.Result,
                Description = eventItem.Description,
                Category = eventItem.Category,
                Teams = eventItem.EventTeams.Select(et => new TeamDto
                {
                    TeamID = et.Team.TeamID,
                    TeamName = et.Team.TeamName
                }).ToList(),
                Odds = eventItem.Odds.Select(o => new OddDto
                {
                    BetType = o.BetType,
                    OddsValue = o.OddsValue,
                    TeamID = o.TeamID,
                    TeamName = o.TeamName
                }).ToList()
            }).ToList();

            return eventDtos;
        }

        public async Task<EventResponseDto> GetEvent(int eventId)
        {
            var eventItem = await _dbContext.Events
                .Include(e => e.Odds)
                .Include(e => e.EventTeams)
                    .ThenInclude(et => et.Team)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventID == eventId);

            if (eventItem is null)
            {
                throw new EntityNotFoundException($"Nie znaleziono eventu o id {eventId}");
            }

            var eventDto = new EventResponseDto
            {
                EventID = eventItem.EventID,
                EventName = eventItem.EventName,
                StartDateTime = eventItem.StartDateTime,
                EventStatus = eventItem.EventStatus,
                Result = eventItem.Result,
                Description = eventItem.Description,
                Category = eventItem.Category,
                Teams = eventItem.EventTeams.Select(et => new TeamDto
                {
                    TeamID = et.Team.TeamID,
                    TeamName = et.Team.TeamName
                }).ToList(),
                Odds = eventItem.Odds.Select(o => new OddDto
                {
                    OddsID = o.OddsID,
                    BetType = o.BetType,
                    OddsValue = o.OddsValue,
                    TeamID = o.TeamID,
                    TeamName = o.TeamName
                }).ToList()
            };
            
            return eventDto;
        }

        public async Task DeleteEvent(int eventId)
        {
             var singleEvent = await _dbContext.Events
                .Include(e => e.Odds)
                .FirstOrDefaultAsync(e => e.EventID == eventId);
            
            if (singleEvent is null)
            {
                throw new EntityNotFoundException($"Event o id:{eventId} nie istnieje!");
            }

            _dbContext.Events.Remove(singleEvent);
            await _dbContext.SaveChangesAsync();
        }

        public string[] GetCategories()
        {
            return Enum.GetNames(typeof(Category));
        }
    }
}
