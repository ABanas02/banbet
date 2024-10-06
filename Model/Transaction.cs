using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace banbet.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserID { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactionDateTime { get; set; } = DateTime.UtcNow;

        public string Description { get; set; }

        public User User { get; set; }
    }

    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Win,
        Loss
    }
}
