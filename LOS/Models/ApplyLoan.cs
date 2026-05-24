using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOS.Models
{
    public class ApplyLoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LoanId { get; set; }

        [Required]

        public int CustomerId { get; set; }   // FK → Customer

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }   // Navigation property

        [Required, StringLength(50)]
        public string LoanType { get; set; }

        [Required]
        public decimal InterestRate { get; set; }

        [Required(ErrorMessage = "Requested amount is required")]
        [Range(1000, 10000000, ErrorMessage = "Requested amount must be between ₹1,000 and ₹1,00,00,000")]
        public decimal RequestedAmount { get; set; }

        public decimal ApprovedAmount { get; set; } = 0;

        [Required, Range(6, 360)]
        public int Tenure { get; set; }

        public decimal Emi { get; set; } // original EMI (requested amount)
        public decimal NewEmi { get; set; } // recalculated EMI (approved amount)

        [Required]
        public string Status { get; set; } = "Applied";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // ✅ Reverse navigation: one loan can have many deals
        public List<Deal> Deals { get; set; }

        public string? Remarks { get; set; }
    }
}
