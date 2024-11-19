using System;
using System.Collections.Generic;

namespace banbet.Models.DTOs
{
    public class EventResponseDto
    {
        public int EventID { get; set; }
        public string? EventName { get; set; }
        public DateTime StartDateTime { get; set; }
        public EventStatus EventStatus { get; set; }
        public Category Category { get; set; }
        public string? Result { get; set; }
        public string? Description { get; set; }

        public List<TeamDto>? Teams { get; set; }
        public List<OddDto>? Odds { get; set; }
    }
}
