using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace banbet.Models
{
    public class Team
    {
        [Key]
        public int TeamID { get; set; }

        [Required]
        public string TeamName { get; set; }

        public string League { get; set; }

        public string Country { get; set; }

        public string LogoUrl { get; set; }

        public ICollection<Player> Players { get; set; }
    }
}
