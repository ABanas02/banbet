using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace banbet.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Hasło zaszyfrowane

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public decimal VirtualBalance { get; set; } = 0;

        public bool IsIdentityVerified { get; set; } = false;

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginDate { get; set; }

        public ICollection<Bet> Bets { get; set; }

        public ICollection<IdentityDocument> IdentityDocuments { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
