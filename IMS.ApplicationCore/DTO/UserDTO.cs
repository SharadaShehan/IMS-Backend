using System.ComponentModel.DataAnnotations;

namespace IMS.ApplicationCore.DTO
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ContactNumber { get; set; }

        [RegularExpression(@"^(Clerk|Technician|Student|AcademicStaff|SystemAdmin)$", ErrorMessage = "ADD ERROR MESSAGE")]
        public string Role { get; set; }
    }
}
