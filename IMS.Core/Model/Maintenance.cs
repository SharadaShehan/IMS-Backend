using System.ComponentModel.DataAnnotations;

namespace IMS.Core.Model
{
    public class Maintenance
    {
        [Required]
        [Key]
        public int MaintenanceId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public Item Item { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int CreatedClerkId { get; set; }

        [Required]
        public User CreatedClerk { get; set; }

        [Required]
        public string TaskDescription { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public int TechnicianId { get; set; }

        [Required]
        public User Technician { get; set; }
        public string? SubmitNote { get; set; }
        public DateTime? SubmittedAt { get; set; }

        public int? ReviewedClerkId { get; set; }
        public User? ReviewedClerk { get; set; }
        public string? ReviewNote { get; set; }
        public DateTime? ReviewedAt { get; set; }

        public int? Cost { get; set; }

        [Required]
        [RegularExpression(
            @"^(Scheduled|Ongoing|UnderReview|Completed|Canceled)$",
            ErrorMessage = "Invalid Maintenance Status"
        )]
        public string Status { get; set; }

        [Required]
        public Boolean IsActive { get; set; }
    }
}
