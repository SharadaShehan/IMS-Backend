using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IMS.ApplicationCore.DTO
{
    public class RequestEquipmentDTO
    {
        public JsonElement jsonElement { get; set; }
        public int equipmentId { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        private string datePattern = @"^\d{4}-\d{2}-\d{2}$";
        public RequestEquipmentDTO(JsonElement jsonElement)
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
                startDate = jsonElement.GetProperty("startDate").ToString(); if (!Regex.IsMatch(startDate, datePattern)) { return new ValidationDTO("Invalid Start Date"); }
                endDate = jsonElement.GetProperty("endDate").ToString(); if (!Regex.IsMatch(endDate, datePattern)) { return new ValidationDTO("Invalid End Date"); }
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
