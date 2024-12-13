using banbet.Data;
using banbet.Models;
using banbet.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace banbet.Services
{
    public class FirstStartupSetupService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly EventsService _eventsService;
        private readonly TeamService _teamService;
        public FirstStartupSetupService
        (
            ApplicationDbContext dbContext,
            EventsService eventsService,
            TeamService teamService
        )
        {
            _dbContext = dbContext;
            _eventsService = eventsService;
            _teamService = teamService;
        }

        public async Task CreateAdmin() 
        {
            var doesExist = await _dbContext.Users.AnyAsync(u => u.Username == "admin");

            if (!doesExist) {
                var admin = new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                    Email = "admin@admin",
                    Role = "Admin"
                };

                _dbContext.Users.Add(admin);

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task CreateEvents()
        {
            
            var eventDto1 = new EventDto {
                EventName = "Mecz piłkarski 1",
                StartDateTime = new DateTime(2024, 12, 8, 14, 30, 0),
                Category = Category.Football,
                Description = "Przykładowy opis :)"
            };

            var eventDto2 = new EventDto {
                EventName = "Mecz piłkarski 2",
                StartDateTime = new DateTime(2024, 12, 8, 14, 30, 0),
                Category = Category.Football,
                Description = "Przykładowy opis :)"
            };
            
            var eventDto3 = new EventDto {
                EventName = "Mecz koszykarski 1",
                StartDateTime = new DateTime(2024, 12, 8, 14, 30, 0),
                Category = Category.Basketball,
                Description = "Przykładowy opis :)"
            };

            var eventDtos = new List<EventDto>
            {
                eventDto1,
                eventDto2,
                eventDto3
            };

            foreach(var eventDto in eventDtos) {
                if (!await _dbContext.Events.AnyAsync(e => e.EventName == eventDto.EventName))
                {
                    await _eventsService.CreateEvent(eventDto);
                };
            }
        }

        public async Task CreateTeams() {

            var team1 = new TeamDto {
                TeamName = "FC Barcelona"
            };

            var team2 = new TeamDto {
                TeamName = "Real Madryt"
            };

            var team3 = new TeamDto {
                TeamName = "AC Milan"
            };

            var team4 = new TeamDto {
                TeamName = "Inter Mediolan"
            };

            var teams = new List<TeamDto>
            {
                team1,
                team2,
                team3,
                team4
            };

            foreach(var team in teams)
            {
                if (!await _dbContext.Teams.AnyAsync(t => t.TeamName == team.TeamName))
                {
                    await _teamService.AddTeam(team);
                }
            }
        }

        public async Task FirstStartupDataSetup() {
            await CreateAdmin();
            await CreateEvents();
            await CreateTeams();
        }
        

    }
}