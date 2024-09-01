using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.ApplicationCore.Model
{
	public class Item
	{
		[Key]
		public int ItemId { get; set; }
		[ForeignKey("Equipment")]
		public int EquipmentId { get; set; }
		public Equipment? Equipment { get; set; }
		public int SerialNumber { get; set; }
		[RegularExpression(@"^(Available|PendingRepair|UnderRepair )$", ErrorMessage = "ADD ERROR MESSAGE")]
		public string? Status { get; set; } 
		public Boolean IsActive { get; set; }
	}
}
