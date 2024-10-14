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
        public DateTime BirthDate { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Role { get; set; } = "User"; // Domyślnie "User"

        public decimal VirtualBalance { get; set; } = 1000.00m;

        public bool IsIdentityVerified { get; set; } = false;

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginDate { get; set; }

        public ICollection<Bet> Bets { get; set; }

        public ICollection<IdentityDocument> IdentityDocuments { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
