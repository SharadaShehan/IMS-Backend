using System.ComponentModel.DataAnnotations;

namespace IMS.Application.DTO
{
    public class MaintenanceDTO
    {
        [Required]
        public int maintenanceId { get; set; }

        [Required]
        public int itemId { get; set; }

        [Required]
        public string itemName { get; set; }

        [Required]
        public string itemModel { get; set; }
        public string? imageUrl { get; set; }

        [Required]
        public string itemSerialNumber { get; set; }

        [Required]
        public int labId { get; set; }

        [Required]
        public string labName { get; set; }

        [Required]
        public DateTime startDate { get; set; }

        [Required]
        public DateTime endDate { get; set; }

        [Required]
        public DateTime createdAt { get; set; }
        public DateTime? submittedAt { get; set; }
        public DateTime? reviewedAt { get; set; }

        [Required]
        [RegularExpression(
            @"^(Scheduled|Ongoing|UnderReview|Completed|Canceled)$",
            ErrorMessage = "Invalid Maintenance Status"
        )]
        public string status { get; set; }
    }
}
