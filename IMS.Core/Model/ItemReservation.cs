using System.ComponentModel.DataAnnotations;

namespace IMS.Core.Model
{
    public class ItemReservation
    {
        [Required]
        [Key]
        public int ItemReservationId { get; set; }

        [Required]
        public int EquipmentId { get; set; }

        [Required]
        public Equipment Equipment { get; set; }

        public int? ItemId { get; set; }
        public Item? Item { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int ReservedUserId { get; set; }

        [Required]
        public User ReservedUser { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public int? RespondedClerkId { get; set; }
        public User? RespondedClerk { get; set; }
        public string? ResponseNote { get; set; }
        public DateTime? RespondedAt { get; set; }

        public int? LentClerkId { get; set; }
        public User? LentClerk { get; set; }
        public DateTime? BorrowedAt { get; set; }

        public int? ReturnAcceptedClerkId { get; set; }
        public User? ReturnAcceptedClerk { get; set; }
        public DateTime? ReturnedAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        [Required]
        [RegularExpression(
            @"^(Pending|Rejected|Reserved|Borrowed|Returned|Canceled)$",
            ErrorMessage = "Invalid Reservation Status"
        )]
        public string Status { get; set; }

        [Required]
        public Boolean IsActive { get; set; }
    }
}
