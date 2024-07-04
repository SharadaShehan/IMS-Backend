using System.ComponentModel.DataAnnotations;

namespace LabTracker.Model
{
	public class Lab
	{
		[Key] 
		public int LabId { get; set; }
		[Required]
		public int Code { get; set; }
		[Required]
		[RegularExpression(@"^(Computer Lab|Embeded System Lab|IoT Lab)$", ErrorMessage = "ADD ERROR MESSAGE")]
		public string Name { get; set; }
	}
}
