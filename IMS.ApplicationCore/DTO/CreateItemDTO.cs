using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IMS.ApplicationCore.DTO
{
    public class CreateItemDTO
    {
        public JsonElement jsonElement { get; set; }
        [Required]
        public int equipmentId { get; set; }
        [Required]
        public string serialNumber { get; set; }
        private string textPattern = @"^.{5,30}$";
        public CreateItemDTO(JsonElement jsonElement)
        {
            this.jsonElement = jsonElement;
        }
        public ValidationDTO Validate()
        {
            try
            {
                int tempEquipmentId;
                if (jsonElement.GetProperty("equipmentId").TryGetInt32(out tempEquipmentId)) { } else { return new ValidationDTO("Invalid Equipment Id"); }
                equipmentId = tempEquipmentId;
                serialNumber = jsonElement.GetProperty("serialNumber").ToString();
                if (!Regex.IsMatch(serialNumber, textPattern)) { return new ValidationDTO("Invalid SerialNumber"); }
                return new ValidationDTO();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new ValidationDTO(ex.Message);
            }
        }
    }
}
