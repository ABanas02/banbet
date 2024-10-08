using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace banbet.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        public string EventName { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public EventStatus EventStatus { get; set; } = EventStatus.Upcoming;

        public string? Result { get; set; }

        public string? Description { get; set; }

        public ICollection<Bet>? Bets { get; set; }

        public ICollection<Odd>? Odds { get; set; }
    }

    public enum EventStatus
    {
        Upcoming,
        Ongoing,
        Finished
    }
}
