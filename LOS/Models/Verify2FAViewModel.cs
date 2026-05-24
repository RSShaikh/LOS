using System.ComponentModel.DataAnnotations;

namespace LOS.Models
{
    public class Verify2FAViewModel
    {
        [Required]
        public string Email { get; set; } = "";

        [Required]
        public string Code { get; set; } = "";
    }
}