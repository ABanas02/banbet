using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace banbet.Models
{
    public class Odd
    {
        [Key]
        public int OddsID { get; set; }

        [Required]
        [ForeignKey("Event")]
        public int EventID { get; set; }
        public Event Event { get; set; }

        [Required]
        public BetType BetType { get; set; }

        [Required]
        public decimal OddsValue { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public int? TeamID { get; set; }
        public string? TeamName { get; set; }
        public Team Team { get; set; }
    }
}
