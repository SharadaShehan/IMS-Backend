using System.ComponentModel.DataAnnotations;

namespace IMS.Core.Model
{
    public class Equipment
    {
        [Required]
        [Key]
        public int EquipmentId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public int LabId { get; set; }

        [Required]
        public Lab Lab { get; set; }
        public string? ImageURL { get; set; }
        public string? Specification { get; set; }
        public int? MaintenanceIntervalDays { get; set; }

        [Required]
        public Boolean IsActive { get; set; }

        //For Foreign keys in ItemReservations
        [Required]
        public ICollection<ItemReservation> ItemReservations { get; set; }

        //For Foreign keys in Items
        [Required]
        public ICollection<Item> Items { get; set; }
    }
}
