using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace banbet.Models
{
    public class Team
    {
        [Key]
        public int TeamID { get; set; }

        public required string TeamName { get; set; }

        public string? League { get; set; }

        public string? Country { get; set; }
        
        public ICollection<EventTeam> EventTeams { get; set; } = new List<EventTeam>();
    }
}
