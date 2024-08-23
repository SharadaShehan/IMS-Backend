using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabTracker.Model
{
	public class Equipment
	{
		[Key]
		public int EquipmentId { get; set; }
		[Required]
		public string Name { get; set; }
		public string Model { get; set; }
		public string ImageURL { get; set; }
		[ForeignKey("Lab")]
		public int LabId { get; set; }
		public Lab Lab { get; set; }
		public	DateTime MaintenanceInterval { get; set; }
		public Boolean IsActive { get; set; }	
	}
}
