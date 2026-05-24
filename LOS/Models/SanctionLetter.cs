using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOS.Models
{
    public class SanctionLetter
    {
        [Key]
        public int SanctionLetterId { get; set; }

        [ForeignKey("LoanDeals")]
        [Required(ErrorMessage = "Deal Id is required")]
        public int LoanDealId { get; set; }
        public Deal? LoanDeals { get; set; }

        [Required(ErrorMessage = "Loan amount is required")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal LoanAmount { get; set; }

        [Required(ErrorMessage = "Interest Rete is required")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal InterestRate { get; set; }

        [Required(ErrorMessage = "Tenure months are required")]
        public int TenureMonths { get; set; }

        [Required(ErrorMessage = "EMI amount is required")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal EMIAmount { get; set; }

        public string? FilePath { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime SanctionDate { get; set; } = DateTime.Now;
    }
}