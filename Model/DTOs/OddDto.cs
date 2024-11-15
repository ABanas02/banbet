using System;
using banbet.Models;

namespace banbet.Models.DTOs
{
    public class OddDto
    {
        public int OddsID { get; set; }
        public int EventID { get; set; }
        public BetType BetType { get; set; }
        public decimal OddsValue { get; set; }
        public int? TeamID { get; set; }
        public string? TeamName { get; set; }
    }
}
