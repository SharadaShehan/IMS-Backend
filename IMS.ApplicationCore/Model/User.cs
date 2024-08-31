using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.ApplicationCore.Model
{
	public class User
	{
		[Key]
		public int UserId { get; set; }
		public string? FirstName { get; set; } 
		public string? LastName { get; set; } 
		public string? Email { get; set; } 
		[Required]
		[RegularExpression(@"^(Clerk|Technician|Student|AcademicStaff|SystemAdmin)$", ErrorMessage = "ADD ERROR MESSAGE")]
		public string? Role { get; set; } 
		public Boolean IsActive { get; set; }


		//For Foreign keys in Maintenace
		public ICollection<Maintenance>? MaintenancesAssignedTechnician { get; set; }
		public ICollection<Maintenance>? MaintenancesAssignedBy { get; set; }
		public ICollection<Maintenance>? MaintenancesReviewedBy { get; set; }

		//For Foreign keys in ItemReservation 
		public ICollection<ItemReservation>? ReservedItems { get; set; }
		public ICollection<ItemReservation>? ResponseItems { get; set; }
		public ICollection<ItemReservation>? BorrowedItems { get; set; }
		public ICollection<ItemReservation>? ReturnedItems { get; set; }
	}
}
