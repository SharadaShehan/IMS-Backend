using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace team4.LTS.Core.Model.DTO
{
	public class ItemReservationDTO
	{
		public int ItemReservationId { get; set; }
		public int RequstedEquipmentId { get; set; }
		public int AsignedItemId { get; set; }
		public int ReservedBy { get; set; }
		public int ResponseedBy { get; set; }
		public int BorrowedFrom { get; set; }
		public int ReturnedTo { get; set; }
		public string? ResponseNote { get; set; }
		public DateTime FromDate { get; set; }
		public DateTime ToDate { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime ResponedAtAt { get; set; }
		public DateTime BorrowedAt { get; set; }
		public DateTime ReturnedAt { get; set; }
		public DateTime CancelledAt { get; set; }
		[RegularExpression(@"^(Pending|Rejected|Reserved|Canceled)$", ErrorMessage = "ADD ERROR MESSAGE")]
		public string? Status { get; set; }
		public Boolean IsActive { get; set; }
	}
}
