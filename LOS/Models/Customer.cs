using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOS.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        public string CustomerCode { get; set; } = "";

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Email address is required.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; } = "";

        [RegularExpression(@"^[6-9]\d{9}$",
            ErrorMessage = "Mobile number must be a valid 10 digit Indian mobile number.")]
        [StringLength(10, MinimumLength = 10,
            ErrorMessage = "Mobile number must be exactly 10 digits.")]
        public string? Mobile { get; set; }

        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$",
            ErrorMessage = "PAN number must be in valid format. Example: ABCDE1234F")]
        [StringLength(10, MinimumLength = 10,
            ErrorMessage = "PAN number must be exactly 10 characters.")]
        public string? PAN { get; set; }

        [RegularExpression(@"^[2-9]{1}[0-9]{11}$",
            ErrorMessage = "Aadhaar number must be 12 digits and should not start with 0 or 1.")]
        [StringLength(12, MinimumLength = 12,
            ErrorMessage = "Aadhaar number must be exactly 12 digits.")]
        public string? AadhaarNo { get; set; }

        public int? Age { get; set; }

        public string? EmploymentType { get; set; }

        [Range(1, 999999999, ErrorMessage = "Monthly income must be greater than 0.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MonthlyIncome { get; set; }

        [Range(0, 999999999, ErrorMessage = "Existing EMI cannot be negative.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ExistingEMI { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string PasswordHash { get; set; } = "";

        public string? OtpCode { get; set; }

        public DateTime? OtpExpiry { get; set; }

        [Required]
        public string Role { get; set; } = "Customer";

        public List<KYCDocument>? KYCDocument { get; set; }
        public List<CibilReport>? CibilReport { get; set; }
        public List<ScoreCard>? ScoreCard { get; set; }
        public List<EligibilityResult>? EligibilityResult { get; set; }
        public List<Deal>? LoanDeals { get; set; }
        public List<DealReview>? DealReviews { get; set; }
        public List<ApplyLoan>? ApplyLoans { get; set; }
    }
}
