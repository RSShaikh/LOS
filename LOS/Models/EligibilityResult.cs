using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOS.Models
{
    public class EligibilityResult
    {
        [Key]
        public int EligibilityId { get; set; }

        [ForeignKey("Customer")]
        [Required(ErrorMessage = "Customer id is required.")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [Required(ErrorMessage = "Cibil score is required.")]
        [Range(300, 900)]
        public int CibilScore { get; set; }

        public bool? IsEligible { get; set; }

        public string? RejectionReason { get; set; }

        public string? ReviewStatus { get; set; }

    }
}
