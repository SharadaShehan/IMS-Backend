using System.ComponentModel.DataAnnotations;

namespace IMS.Application.DTO
{
    public class ItemDetailedDTO
    {
        [Required]
        public int itemId { get; set; }

        [Required]
        public string itemName { get; set; }

        [Required]
        public string itemModel { get; set; }
        public string? imageUrl { get; set; }

        [Required]
        public int equipmentId { get; set; }

        [Required]
        public int labId { get; set; }

        [Required]
        public string labName { get; set; }

        [Required]
        public string serialNumber { get; set; }
        public DateTime? lastMaintenanceOn { get; set; }
        public string? lastMaintenanceBy { get; set; }

        [Required]
        [RegularExpression(
            @"^(Available|Borrowed|UnderRepair|Unavailable)$",
            ErrorMessage = "Invalid Item Status"
        )]
        public string status { get; set; }
    }
}
