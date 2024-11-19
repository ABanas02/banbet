using System;
using System.ComponentModel.DataAnnotations;

namespace banbet.Models.DTOs
{
    public class EventDto
    {
        [Required]
        public string EventName { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        public Category Category { get; set; }

        public string Description { get; set; }

    }
}