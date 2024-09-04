using System.ComponentModel.DataAnnotations;

namespace IMS.ApplicationCore.DTO
{
    public class EquipmentDTO
    {
        [Required]
        public int equipmentId { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string model { get; set; }
        [Required]
        public int labId { get; set; }
        public string? imageURL { get; set; }
        public string? specification { get; set; }
        public int? maintenanceIntervalDays { get; set; }
    }
}
