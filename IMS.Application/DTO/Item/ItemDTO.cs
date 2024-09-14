using System.ComponentModel.DataAnnotations;

namespace IMS.Application.DTO
{
    public class ItemDTO
    {
        [Required]
        public int itemId { get; set; }
        public string? imageUrl { get; set; }

        [Required]
        public int equipmentId { get; set; }

        [Required]
        public string serialNumber { get; set; }

        [Required]
        [RegularExpression(
            @"^(Available|Borrowed|UnderRepair|Unavailable)$",
            ErrorMessage = "Invalid Item Status"
        )]
        public string status { get; set; }
    }
}
