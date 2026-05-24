using System.ComponentModel.DataAnnotations;

namespace LOS.Models
{
    public class DealReview
    {
      
            [Key]
            public int ReviewId { get; set; }

            [Required]
            public int DealId { get; set; }   // FK → Deal

            [Required]
            public int OfficerId { get; set; }   // FK → Officer table (if added)
            public string OfficerName { get; set; } = "Test Officer"; // hardcoded for now

           [Required]
            public string Decision { get; set; } // Approved/Rejected/Modified

            public string Remarks { get; set; }

            public DateTime ReviewDate { get; set; } = DateTime.Now;

           public Deal Deal { get; set; }


    }
}
