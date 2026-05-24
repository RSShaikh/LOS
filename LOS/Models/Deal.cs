using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOS.Models
{
    public class Deal
    {
        [Key]
        public int DealId { get; set; }

        [Required]
        public int LoanId { get; set; }   // FK → ApplyLoan

        [ForeignKey("LoanId")]
        public ApplyLoan Loan { get; set; }   // Navigation property

        public decimal RequestedAmount { get; set; }   // mirror from ApplyLoan
        public decimal ApprovedAmount { get; set; } = 0;


        [Required, Range(6, 360)]
        public int Tenure { get; set; }

        [Required, Range(1, 20)]
        public decimal InterestRate { get; set; }
        
        [Required, Range(1000, 10000000)]
        public decimal EMI { get; set; }


        [Required, StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending/Accepted/Rejected/Modified

        public SanctionLetter? SanctionLetter { get; set; }
        public Disbursement? Disbursement { get; set; }

        public List<DealReview>? DealReviews { get; set; }





    }
}
