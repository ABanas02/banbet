using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace banbet.Models.DTOs
{
    public class AddTeamsToEventDto
    {
        [Required]
        public int EventID { get; set; }

        [Required]
        public List<int> TeamIDs { get; set; }
    }
}
