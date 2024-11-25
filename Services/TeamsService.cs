using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using banbet.Data;
using banbet.Models;
using banbet.Models.DTOs;
using banbet.CustomExceptions;

namespace banbet.Services
{
    public class TeamService
    {
        private readonly ApplicationDbContext _dbContext;

        public TeamService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Team>> GetTeams()
        {
            var teams = await _dbContext.Teams.ToListAsync();

            if (teams == null || teams.Count == 0)
            {
                throw new EntityNotFoundException("Nie znaleziono żadnych drużyn.");
            }

            return teams;
        }

        public async Task<Team> GetTeam(int id)
        {
            var team = await _dbContext.Teams.FindAsync(id);

            if (team == null)
            {
                throw new EntityNotFoundException($"Nie znaleziono drużyny o ID {id}.");
            }

            return team;
        }

        public async Task<Team> AddTeam(TeamDto teamDto)
        {
            if (teamDto == null || string.IsNullOrWhiteSpace(teamDto.TeamName))
            {
                throw new ArgumentException("Nieprawidłowe dane drużyny.");
            }

            var team = new Team
            {
                TeamName = teamDto.TeamName
            };

            _dbContext.Teams.Add(team);
            await _dbContext.SaveChangesAsync();

            return team;
        }

        public async Task<List<TeamDto>> GetTeamsFromEvent(int eventID)
        {
            var eventExists = await _dbContext.Events.AnyAsync(e => e.EventID == eventID);
            if (!eventExists)
            {
                throw new EntityNotFoundException($"Wydarzenie o ID {eventID} nie istnieje.");
            }

            var teams = await _dbContext.EventTeams
                                        .Where(et => et.EventID == eventID)
                                        .Select(et => new TeamDto
                                        {
                                            TeamID = et.Team.TeamID,
                                            TeamName = et.Team.TeamName
                                        })
                                        .AsNoTracking()
                                        .ToListAsync();

            if (teams == null || teams.Count == 0)
            {
                throw new EntityNotFoundException($"Nie znaleziono drużyn dla wydarzenia o ID {eventID}.");
            }

            return teams;
        }

        public async Task DeleteTeam(int id)
        {
            var team = await _dbContext.Teams.FindAsync(id);

            if (team == null)
            {
                throw new EntityNotFoundException($"Nie znaleziono drużyny z ID {id}.");
            }

            _dbContext.Teams.Remove(team);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> AddTeamsToEvent(AddTeamsToEventDto dto)
        {
            if (dto == null || dto.TeamIDs == null || dto.TeamIDs.Count == 0)
            {
                throw new ArgumentException("Nieprawidłowe dane wejściowe.");
            }

            var eventItem = await _dbContext.Events.FindAsync(dto.EventID);
            if (eventItem == null)
            {
                throw new EntityNotFoundException($"Wydarzenie o ID {dto.EventID} nie istnieje.");
            }

            foreach (var teamId in dto.TeamIDs)
            {
                var team = await _dbContext.Teams.FindAsync(teamId);
                if (team == null)
                {
                    throw new EntityNotFoundException($"Drużyna o ID {teamId} nie istnieje.");
                }

                var eventTeamExists = await _dbContext.EventTeams
                    .AnyAsync(et => et.EventID == dto.EventID && et.TeamID == teamId);

                if (!eventTeamExists)
                {
                    var eventTeam = new EventTeam
                    {
                        EventID = dto.EventID,
                        TeamID = teamId
                    };

                    _dbContext.EventTeams.Add(eventTeam);
                }
            }

            await _dbContext.SaveChangesAsync();

            return $"Drużyny zostały dodane do wydarzenia o ID {dto.EventID}.";
        }
    }
}
