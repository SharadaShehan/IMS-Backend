using System.ComponentModel.DataAnnotations;

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
	}
}
