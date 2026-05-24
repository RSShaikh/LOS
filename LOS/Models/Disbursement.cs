using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOS.Models
{
    public class Disbursement
    {
        [Key]
        public int DisbursementId { get; set; }

        [ForeignKey("LoanDeals")]
        [Required(ErrorMessage = "Deal Id is required")]
        public int LoanDealId { get; set; }
        public Deal? LoanDeals { get; set; }

        [Required(ErrorMessage ="Disbursement amount is required")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DisbursedAmount { get; set; }

        [Required(ErrorMessage ="Bank name is required")]
        public string BankName { get; set; }

        [Required]
        [StringLength(50)]
        public string DisbursementStatus { get; set; } = "Pending";

        public DateTime DisbursementDate { get; set; } = DateTime.Now;
    }
}
