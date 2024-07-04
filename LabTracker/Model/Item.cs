using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabTracker.Model
{
	public class Item
	{
		[Key] 
		public int ItemId { get; set; }
		[ForeignKey("Equipment")]
		public int EquipmentId { get; set; }
		public Equipment Equipment { get; set; }
		[ForeignKey("Lab")]
		public int LabId { get; set; }
		public Lab Lab { get; set; }
		public int SerialNumber { get; set; }
		[RegularExpression(@"^(Available|PendingRepair|UnderRepair )$", ErrorMessage = "ADD ERROR MESSAGE")]
		public string Status { get; set; }
	}
}
