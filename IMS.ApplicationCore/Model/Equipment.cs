using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.ApplicationCore.Model
{
	public class Equipment
	{
		[Key]
		public int EquipmentId { get; set; }
		[Required]
		public string? Name { get; set; } 
		public string? ImageURL { get; set; }
		public string? Model {get; set; }
		public string? Specification { get; set; }
		public int LabId { get; set; }
		public Lab? Lab { get; set; } 
		public DateTime MaintenanceInterval { get; set; }
		public Boolean IsActive { get; set; }
	}
}
