using System.ComponentModel.DataAnnotations;

namespace LOS.Models
{
    public class VerifyOtpViewModel
    {
        [Required]
        public string Email { get; set; } = "";

        [Required]
        public string OtpCode { get; set; } = "";
    }
}
