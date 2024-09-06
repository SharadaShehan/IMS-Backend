using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IMS.ApplicationCore.DTO
{
    public class CreateMaintenanceDTO
    {
        public JsonElement jsonElement { get; set; }
        public int itemId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int technicianId { get; set; }
        public string taskDescription { get; set; }
        private string datePattern = @"^\d{4}-\d{2}-\d{2}$";
        private string discriptionPattern = @"^.{1,100}$";
        public CreateMaintenanceDTO(JsonElement jsonElement)
        {
            this.jsonElement = jsonElement;
        }
        public ValidationDTO Validate()
        {
            try
            {
                int tempItemId, tempTechnicianId;
                if (jsonElement.GetProperty("itemId").TryGetInt32(out tempItemId)) { } else { return new ValidationDTO("Invalid Item Id"); }
                if (jsonElement.GetProperty("technicianId").TryGetInt32(out tempTechnicianId)) { } else { return new ValidationDTO("Invalid Technician Id"); }
                itemId = tempItemId; technicianId = tempTechnicianId;
                string tempStartDate = jsonElement.GetProperty("startDate").ToString(); if (!Regex.IsMatch(tempStartDate, datePattern)) { return new ValidationDTO("Invalid Start Date"); } else { startDate = DateTime.Parse(tempStartDate); }
                string tempEndDate = jsonElement.GetProperty("endDate").ToString(); if (!Regex.IsMatch(tempEndDate, datePattern)) { return new ValidationDTO("Invalid End Date"); } else { endDate = DateTime.Parse(tempEndDate); }
                taskDescription = jsonElement.GetProperty("taskDescription").ToString(); if (!Regex.IsMatch(taskDescription, discriptionPattern)) { return new ValidationDTO("Invalid Task Description"); }
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
