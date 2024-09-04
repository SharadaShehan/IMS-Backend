using System.ComponentModel.DataAnnotations;

namespace IMS.ApplicationCore.DTO
{
    public class ItemReservationDTO
    {
        [Required]
        public int itemReservationId { get; set; }
        [Required]
        public int equipmentId { get; set; }
        public int? itemId { get; set; }
        [Required]
        public DateTime startDate { get; set; }
        [Required]
        public DateTime endDate { get; set; }
        [Required]
        public int reservedUserId { get; set; }
        [Required]
        public DateTime createdAt { get; set; }

        public int? respondedClerkId { get; set; }
        public string? responseNote { get; set; }
        public DateTime? respondedAt { get; set; }

        public int? lentClerkId { get; set; }
        public DateTime? borrowedAt { get; set; }

        public int? returnAcceptedClerkId { get; set; }
        public DateTime? returnedAt { get; set; }
        public DateTime? cancelledAt { get; set; }

        [Required]
        [RegularExpression(@"^(Pending|Rejected|Reserved|Borrowed|Returned|Canceled)$", ErrorMessage = "Invalid Reservation Status")]
        public string status { get; set; }
    }
}
