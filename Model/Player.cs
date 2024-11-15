using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace banbet.Models
{
    public class Player
    {
        [Key]
        public int PlayerID { get; set; }

        [Required]
        public string FullName { get; set; }

        [ForeignKey("Team")]
        public int? TeamID { get; set; }

        public string Position { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Team Team { get; set; }
    }
}
