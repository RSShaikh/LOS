using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOS.Models
{
    public class ScoreCard
    {
        [Key]
        public int ScoreCardId { get; set; }

        [ForeignKey("Customer")]
        [Required(ErrorMessage = "Customer id is required.")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [Required(ErrorMessage = "Risk Category is required")]
        public string RiskCategory { get; set; }

        [Required(ErrorMessage = "Enter loan amount")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal EligibleLoanAmount { get; set; }

        public bool IsRiskOverridden { get; set; } = false;


        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
