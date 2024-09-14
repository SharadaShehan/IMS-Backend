using System.ComponentModel.DataAnnotations;

namespace IMS.Core.Model
{
    public class User
    {
        [Required]
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string ContactNumber { get; set; }

        [Required]
        [RegularExpression(
            @"^(Clerk|Technician|Student|AcademicStaff|SystemAdmin)$",
            ErrorMessage = "Invalid Role Name"
        )]
        public string Role { get; set; }

        [Required]
        public Boolean IsActive { get; set; }

        //For Foreign keys in Maintenace
        [Required]
        public ICollection<Maintenance> MaintenancesAssignedTo { get; set; }

        [Required]
        public ICollection<Maintenance> MaintenancesCreatedBy { get; set; }

        [Required]
        public ICollection<Maintenance> MaintenancesReviewedBy { get; set; }

        //For Foreign keys in ItemReservation
        [Required]
        public ICollection<ItemReservation> ItemsReservedBy { get; set; }

        [Required]
        public ICollection<ItemReservation> ReservationsRespondedTo { get; set; }

        [Required]
        public ICollection<ItemReservation> ItemsBorrowedFrom { get; set; }

        [Required]
        public ICollection<ItemReservation> ItemsReturnedTo { get; set; }
    }
}
