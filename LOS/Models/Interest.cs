using System.ComponentModel.DataAnnotations;

namespace LOS.Models
{
    public class Interest
    {
        [Key]
        public int InterestId { get; set; }

        [Required, StringLength(50)]
        public string LoanType { get; set; }   // e.g. Home Loan, Car Loan, Personal Loan

        [Required, Range(1, 20)]
        public decimal InterestRate { get; set; }   // Annual interest rate in %
    }
}
