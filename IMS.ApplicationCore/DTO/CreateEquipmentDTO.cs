using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IMS.ApplicationCore.DTO
{
    public class CreateEquipmentDTO
    {
        public JsonElement jsonElement { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string model { get; set; }
        [Required]
        public int labId { get; set; }
        public string? imageURL { get; set; }
        public string? specification { get; set; }
        public int? maintenanceIntervalDays { get; set; }
        private string textPattern = @"^.{2,20}$";
        private string imageUrlPattern = @"^https?:\/\/.*\.(?:png|jpg|jpeg|webp)$";
        public CreateEquipmentDTO(JsonElement jsonElement)
        {
            this.jsonElement = jsonElement;
        }
        public ValidationDTO Validate()
        {
            try
            {
                int tempLabId;
                if (jsonElement.GetProperty("labId").TryGetInt32(out tempLabId)) { } else { return new ValidationDTO("Invalid Lab Id"); }
                labId = tempLabId;
                name = jsonElement.GetProperty("name").ToString(); if (!Regex.IsMatch(name, textPattern)) { return new ValidationDTO("Invalid Equipment Name"); }
                model = jsonElement.GetProperty("model").ToString(); if (!Regex.IsMatch(model, textPattern)) { return new ValidationDTO("Invalid Equipment Model"); }
                if (imageURL != null) {
                    imageURL = jsonElement.GetProperty("imageURL").ToString();
                    if (!Regex.IsMatch(imageURL, imageUrlPattern)) { return new ValidationDTO("Invalid Image Url"); }
                }
                if (specification != null) {
                    specification = jsonElement.GetProperty("specification").ToString();
                }
                if (maintenanceIntervalDays != null) {
                    int tempMaintenanceIntervalDays;
                    if (jsonElement.GetProperty("maintenanceIntervalDays").TryGetInt32(out tempMaintenanceIntervalDays)) { } else { return new ValidationDTO("Invalid Maintenance Interval Days"); }
                    maintenanceIntervalDays = tempMaintenanceIntervalDays;
                }
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
