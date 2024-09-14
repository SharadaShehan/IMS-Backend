using System.ComponentModel.DataAnnotations;

namespace IMS.Core.Model
{
    public class Item
    {
        [Required]
        [Key]
        public int ItemId { get; set; }

        [Required]
        public int EquipmentId { get; set; }

        [Required]
        public Equipment Equipment { get; set; }

        [Required]
        public string SerialNumber { get; set; }

        [Required]
        [RegularExpression(
            @"^(Available|Borrowed|UnderRepair|Unavailable)$",
            ErrorMessage = "Invalid Item Status"
        )]
        public string Status { get; set; }

        [Required]
        public Boolean IsActive { get; set; }

        //For Foreign keys in Maintenance
        [Required]
        public ICollection<Maintenance> Maintenances { get; set; }

        //For Foreign keys in Reservations
        [Required]
        public ICollection<ItemReservation> Reservations { get; set; }
    }
}
