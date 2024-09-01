using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.ApplicationCore.Model
{
	public class ItemReservation
	{
		[Key]
		public int ItemReservationId { get; set; }
		[Required]
		[ForeignKey("Equipment")]
		public int RequstedEquipmentId { get; set; }
		public Equipment? Equipment { get; set; }
		[Required]
		[ForeignKey("Item")]
		public int AsignedItemId { get; set; }
		public Item? Item { get; set; }




		[Required]
		public int ReservedBy { get; set; }
		public int ResponseedBy { get; set; }
		public int BorrowedFrom { get; set; }
		public int ReturnedTo { get; set; }
		[ForeignKey(nameof(ReservedBy))]
		public User? ReservedByUser { get; set; }

		[ForeignKey(nameof(ResponseedBy))]
		public User? ResponseedByUser { get; set; }

		[ForeignKey(nameof(BorrowedFrom))]
		public User? BorrowedFromUser { get; set; }

		[ForeignKey(nameof(ReturnedTo))]
		public User? ReturnedToUser { get; set; }



		public string? ResponseNote { get; set; } 
		[Required]
		public DateTime FromDate { get; set; }
		[Required]
		public DateTime ToDate { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime ResponedAtAt { get; set;}
		public DateTime BorrowedAt{ get; set;}
		public DateTime ReturnedAt { get; set;}
		public DateTime CancelledAt { get; set;}
		[Required]
		[RegularExpression(@"^(Pending|Rejected|Reserved|Canceled)$", ErrorMessage = "ADD ERROR MESSAGE")]
		public string? Status { get; set; } 
		public Boolean IsActive { get; set; }
	}
}
