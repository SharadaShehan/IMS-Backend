using System.ComponentModel.DataAnnotations;
using IMS.Core.Model;

namespace IMS.Application.DTO
{
    public class ItemReservationDTO
    {
        [Required]
        [Key]
        public int reservationId { get; set; }

        [Required]
        public int equipmentId { get; set; }

        [Required]
        public string itemName { get; set; }

        [Required]
        public string itemModel { get; set; }
        public string? imageUrl { get; set; }
        public int? itemId { get; set; }
        public string? itemSerialNumber { get; set; }

        [Required]
        public int labId { get; set; }

        [Required]
        public string labName { get; set; }

        [Required]
        public DateTime startDate { get; set; }

        [Required]
        public DateTime endDate { get; set; }

        [Required]
        public int reservedUserId { get; set; }

        [Required]
        public string reservedUserName { get; set; }

        [Required]
        public DateTime createdAt { get; set; }
        public DateTime? respondedAt { get; set; }
        public DateTime? borrowedAt { get; set; }
        public DateTime? returnedAt { get; set; }

        public DateTime? cancelledAt { get; set; }

        [Required]
        [RegularExpression(
            @"^(Pending|Rejected|Reserved|Borrowed|Returned|Canceled)$",
            ErrorMessage = "Invalid Reservation Status"
        )]
        public string status { get; set; }
    }
}
