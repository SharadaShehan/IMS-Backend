using System.ComponentModel.DataAnnotations;

namespace IMS.Application.DTO
{
    public class PendingMaintenanceDTO
    {
        [Required]
        public int itemId { get; set; }

        [Required]
        public string itemName { get; set; }

        [Required]
        public string itemModel { get; set; }

        [Required]
        public string itemSerialNumber { get; set; }
        public string? imageUrl { get; set; }

        [Required]
        public int labId { get; set; }

        [Required]
        public string labName { get; set; }

        [Required]
        public int lastMaintenanceId { get; set; }

        [Required]
        public DateTime lastMaintenanceStartDate { get; set; }

        [Required]
        public DateTime lastMaintenanceEndDate { get; set; }
    }
}
