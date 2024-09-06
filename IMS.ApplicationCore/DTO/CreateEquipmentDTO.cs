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
                if (jsonElement.TryGetProperty("imageURL", out JsonElement imageURLElm))
                {
                    if (imageURLElm.GetType() == typeof(string))
                    {
                        imageURL = imageURLElm.GetString();
                        if (!Regex.IsMatch(imageURL, imageUrlPattern)) { return new ValidationDTO("Invalid Image Url"); }
                    }
                    else
                    {
                        return new ValidationDTO("Invalid Image Url");
                    }
                }
                if (jsonElement.TryGetProperty("specification", out JsonElement specificationElm))
                {
                    if (specificationElm.GetType() == typeof(string)) specification = specificationElm.GetString();
                    else return new ValidationDTO("Invalid Specification");
                }
                if (jsonElement.TryGetProperty("maintenanceIntervalDays", out JsonElement mIntervalDays))
                {
                    if (mIntervalDays.TryGetInt32(out int tempMaintenanceIntervalDays))
                    {
                        maintenanceIntervalDays = tempMaintenanceIntervalDays;
                    }
                    else
                    {
                        return new ValidationDTO("Invalid Maintenance Interval Days");
                    }
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
