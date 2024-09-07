using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;

namespace IMS.ApplicationCore.DTO
{
    public class CreateItemDTO
    {
        public int equipmentId { get; set; }
        public string serialNumber { get; set; }
        
    }
}
