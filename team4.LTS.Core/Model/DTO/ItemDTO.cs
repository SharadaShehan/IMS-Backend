using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace team4.LTS.Core.Model.DTO
{
	public class ItemDTO
	{
		public int ItemId { get; set; }
		public int EquipmentId { get; set; }
		public int SerialNumber { get; set; }
		[RegularExpression(@"^(Available|PendingRepair|UnderRepair )$", ErrorMessage = "ADD ERROR MESSAGE")]
		public string? Status { get; set; }
		public Boolean IsActive { get; set; }
	}
}
