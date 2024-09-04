using System.ComponentModel.DataAnnotations;

namespace IMS.ApplicationCore.DTO
{
    public class MaintenanceDTO
    {
        [Required]
        public int maintenanceId { get; set; }
        [Required]
        public int itemId { get; set; }
        [Required]
        public DateTime startDate { get; set; }
        [Required]
        public DateTime endDate { get; set; }
        [Required]
        public int createdClerkId { get; set; }
        [Required]
        public string taskDescription { get; set; }
        [Required]
        public DateTime createdAt { get; set; }

        [Required]
        public int technicianId { get; set; }
        public string? submitNote { get; set; }
        public DateTime? submittedAt { get; set; }

        public int? reviewedClerkId { get; set; }
        public string? reviewNote { get; set; }
        public DateTime? reviewedAt { get; set; }

        public int? cost { get; set; }
        [Required]
        [RegularExpression(@"^(Ongoing|UnderReview|Completed)$", ErrorMessage = "Invalid Maintenance Status")]
        public string status { get; set; }
    }
}
