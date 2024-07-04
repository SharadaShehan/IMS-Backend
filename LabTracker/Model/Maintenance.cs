using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabTracker.Model
{
	public class Maintenance
	{
		[Key]
		public int MaintenanceId { get; set; }
		[Required]
		[ForeignKey("Item")]
		public int ItemId { get; set; }
		public Item Item { get; set; }
		[Required]
		[ForeignKey("User")]
		public string AssignedTechnician { get; set; }
		public User User { get; set; }
		public string Description { get; set; }
		public DateTime  StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int Cost { get; set; }
		[RegularExpression(@"^(Unassigned|Ongoin|UnderReview|Completed)$",ErrorMessage = "ADD ERROR MESSAGE")]
		public string Status { get; set; }
	}
}
