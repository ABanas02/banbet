using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace banbet.Models
{
    public class Bet
    {
        [Key]
        public int BetID { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserID { get; set; }

        [Required]
        [ForeignKey("Event")]
        public int EventID { get; set; }

        [Required]
        public decimal BetAmount { get; set; }

        [Required]
        [ForeignKey("Odd")]
        public int OddsID { get; set; }
        public Odd Odd { get; set; }

        [Required]
        public BetType BetType { get; set; }

        [Required]
        public decimal OddsAtBetTime { get; set; }

        [Required]
        public BetStatus BetStatus { get; set; } = BetStatus.Open;

        [Required]
        public DateTime BetDate { get; set; } = DateTime.UtcNow;

        public User User { get; set; }

        public Event Event { get; set; }
    }

    public enum BetType
    {
        MatchWinner,  
        TotalGoals, 
        BothTeamsScoreW
    }

    public enum BetStatus
    {
        Open,
        Won,
        Lost
    }
}
