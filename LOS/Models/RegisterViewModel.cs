using System.ComponentModel.DataAnnotations;

namespace LOS.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [RegularExpression(@"^[A-Za-z]+$",
            ErrorMessage = "First name must contain only letters.")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression(@"^[A-Za-z]+$",
            ErrorMessage = "Last name must contain only letters.")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9._%+-]*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Email must start with a letter and be a valid address (e.g. john@gmail.com).")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$",
            ErrorMessage = "Mobile must be a valid 10-digit Indian number starting with 6-9.")]
        public string Mobile { get; set; } = "";

        [Required(ErrorMessage = "PAN number is required.")]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$",
            ErrorMessage = "Invalid PAN. Format: ABCDE1234F (5 letters, 4 digits, 1 letter).")]
        public string PAN { get; set; } = "";

        [Required(ErrorMessage = "Aadhaar number is required.")]
        [RegularExpression(@"^[2-9][0-9]{11}$",
            ErrorMessage = "Aadhaar must be 12 digits and must not start with 0 or 1.")]
        public string AadhaarNo { get; set; } = "";

        [Required(ErrorMessage = "Employment type is required.")]
        public string EmploymentType { get; set; } = "";

        [Required(ErrorMessage = "Monthly income is required.")]
        [Range(1, 999999999, ErrorMessage = "Please enter a valid monthly income.")]
        public decimal MonthlyIncome { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(18, 60, ErrorMessage = "Age must be between 18 and 60 years.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must be 8+ characters with uppercase, lowercase, number, and special character.")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }
}
