using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabTracker.Model
{
	public class User
	{
		[Key]
		public string Email { get; set; }
		[Required]
		[RegularExpression(@"^(Clerk|Technician|Student|AcademicStaff|SystemAdmin)$", ErrorMessage = "ADD ERROR MESSAGE")]
		public string Role { get; set; }
	}
}
