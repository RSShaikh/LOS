using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOS.Models
{
    public class CibilReport
    {
        [Key]
        public int CibilReportId { get; set; }

        [ForeignKey("Customer")]
        [Required(ErrorMessage = "Customer id is required.")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [Required(ErrorMessage = "PAN number is required.")]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "PAN number must be in valid format. Example: ABCDE1234F")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "PAN number must be exactly 10 characters.")]
        public string PAN { get; set; }

        [Required(ErrorMessage = "Cibil score is required.")]
        [Range(300, 900)]
        public int CibilScore { get; set; }

        public DateTime CheckDate { get; set; } = DateTime.Now;
    }
}
