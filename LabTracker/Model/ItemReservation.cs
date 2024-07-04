using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabTracker.Model
{
	public class ItemReservation
	{
		[Key]
		public int ItemReservationId { get; set; }
		[Required]
		[ForeignKey("Item")]
		public int ItemId { get; set; }
		public Item Item { get; set; }
		[Required]
		[ForeignKey("User")]
		public string Email { get; set; }
		public User User { get; set; }
		[Required]
		public DateTime FromDate { get; set; }
		[Required]
		public DateTime ToDate { get; set; }
		[Required]
		[RegularExpression(@"^(Pending|Rejected|Reserved|Canceled)$", ErrorMessage = "ADD ERROR MESSAGE")]
		public string Status { get; set; }

	}
}
