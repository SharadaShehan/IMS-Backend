using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace team4.LTS.Core.Model.DTO
{
	public class EquipmentDTO
	{
		public int EquipmentId { get; set; }
		public string? Name { get; set; }
		public string? ImageURL { get; set; }
		public string? Model { get; set; }
		public string? Specification { get; set; }
		public int LabId { get; set; }
		public DateTime MaintenanceInterval { get; set; }
		public Boolean IsActive { get; set; }
	}
}
