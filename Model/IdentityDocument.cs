using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace banbet.Models
{
    public class IdentityDocument
    {
        [Key]
        public int DocumentID { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserID { get; set; }

        [Required]
        public string DocumentType { get; set; }

        [Required]
        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DocumentStatus VerificationStatus { get; set; } = DocumentStatus.Pending;

        public string Notes { get; set; }

        public User User { get; set; }
    }

    public enum DocumentStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
