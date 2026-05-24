using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOS.Models
{
    public class KYCDocument
    {
        [Key]
        public int DocumentId { get; set; }

        [ForeignKey("Customer")]
        [Required(ErrorMessage = "Customer id is required.")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [Required(ErrorMessage = "Document type is required.")]
        public string DocumentType { get; set; } = "";

        [Required(ErrorMessage = "Upload file")]
        public string FilePath { get; set; } = "";

        public string FileName { get; set; } = "";

        [Required]
        public string VerificationStatus { get; set; } = "Pending";

        public string? RejectReason { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}
