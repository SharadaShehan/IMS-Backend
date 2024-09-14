using System.ComponentModel.DataAnnotations;

namespace IMS.Core.Model
{
    public class Lab
    {
        [Required]
        [Key]
        public int LabId { get; set; }

        [Required]
        public string LabName { get; set; }

        [Required]
        public string LabCode { get; set; }
        public string? ImageURL { get; set; }

        [Required]
        public Boolean IsActive { get; set; }

        //For Foreign keys in Equipments
        [Required]
        public ICollection<Equipment> Equipments { get; set; }
    }
}
