using System.ComponentModel.DataAnnotations;

namespace IMS.Application.DTO
{
    public class UserDTO
    {
        public int userId { get; set; }
        public string email { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? contactNumber { get; set; }

        [RegularExpression(
            @"^(Clerk|Technician|Student|AcademicStaff|SystemAdmin)$",
            ErrorMessage = "ADD ERROR MESSAGE"
        )]
        public string role { get; set; }
    }
}
