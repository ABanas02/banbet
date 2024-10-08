using System;
using System.ComponentModel.DataAnnotations;
using banbet.Models;

namespace banbet.Models.DTOs
{
    public class OddDto
    {
        [Required]
        public int EventID { get; set; }

        [Required]
        public BetType BetType { get; set; }

        [Required]
        public decimal OddsValue { get; set; }
    }
}
