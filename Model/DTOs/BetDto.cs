using System.ComponentModel.DataAnnotations;

namespace banbet.Models.DTOs
{
    public class BetDto
    {
        [Required]
        public int OddsID { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Kwota zakładu musi być większa niż 0.")]
        public decimal BetAmount { get; set; }
    }
}
