using System.ComponentModel.DataAnnotations;

namespace IMS.ApplicationCore.DTO
{
    public class LabDTO
    {
        [Required]
        public int labId { get; set; }
        [Required]
        public string labName { get; set; }
        [Required]
        public string labCode { get; set; }
        public string? ImageURL { get; set; }
    }
}
