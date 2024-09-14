using System.ComponentModel.DataAnnotations;

namespace IMS.Application.DTO
{
    public class EquipmentDetailedDTO
    {
        [Required]
        public int equipmentId { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string model { get; set; }
        public string? imageUrl { get; set; }

        [Required]
        public int labId { get; set; }

        [Required]
        public string labName { get; set; }
        public string? specification { get; set; }
        public int? maintenanceIntervalDays { get; set; }

        [Required]
        public int totalCount { get; set; }

        [Required]
        public int reservedCount { get; set; }

        [Required]
        public int availableCount { get; set; }
    }
}
