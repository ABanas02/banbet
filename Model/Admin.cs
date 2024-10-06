using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace banbet.Models
{
    public class Admin
    {
        [Key]
        public int AdminID { get; set; }

        [Required]
        [MaxLength(50)]
        public string AdminUsername { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Hasło zaszyfrowane

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public AdminRole Role { get; set; } = AdminRole.Moderator;

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    }

    public enum AdminRole
    {
        SuperAdmin,
        Moderator
    }
}
