using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IMS.ApplicationCore.DTO
{
    public class RequestEquipmentDTO
    {
        public JsonElement jsonElement { get; set; }
        public int equipmentId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
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
                string tempStartDate = jsonElement.GetProperty("startDate").ToString(); if (!Regex.IsMatch(tempStartDate, datePattern)) { return new ValidationDTO("Invalid Start Date"); } else { startDate = DateTime.Parse(tempStartDate); }
                string tempEndDate = jsonElement.GetProperty("endDate").ToString(); if (!Regex.IsMatch(tempEndDate, datePattern)) { return new ValidationDTO("Invalid End Date"); } else { endDate = DateTime.Parse(tempEndDate); }
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
