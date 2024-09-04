using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IMS.ApplicationCore.DTO
{
    public class UpdateEquipmentDTO
    {
        public JsonElement jsonElement { get; set; }
        public string? name { get; set; }
        public string? model { get; set; }
        public string? imageURL { get; set; }
        public string? specification { get; set; }
        public int? maintenanceIntervalDays { get; set; }
        private string textPattern = @"^.{2,20}$";
        private string imageUrlPattern = @"^https?:\/\/.*\.(?:png|jpg|jpeg|webp)$";
        public UpdateEquipmentDTO(JsonElement jsonElement)
        {
            this.jsonElement = jsonElement;
        }
        public ValidationDTO Validate()
        {
            try
            {
                if (jsonElement.TryGetProperty("name", out JsonElement nameElm)) {
                    if (nameElm.GetType() == typeof(string))
                    {
                        name = nameElm.GetString();
                        if (!Regex.IsMatch(name, textPattern)) { return new ValidationDTO("Invalid Equipment Name"); }
                    }
                    else return new ValidationDTO("Invalid Equipment Name");
                }
                if (jsonElement.TryGetProperty("model", out JsonElement modelElm)) {
                    if (modelElm.GetType() == typeof(string))
                    {
                        model = modelElm.GetString();
                        if (!Regex.IsMatch(model, textPattern)) { return new ValidationDTO("Invalid Equipment Model"); }
                    }
                    else return new ValidationDTO("Invalid Equipment Model");
                }
                if (jsonElement.TryGetProperty("imageURL", out JsonElement imageURLElm)) {
                    if (imageURLElm.GetType() == typeof(string))
                    {
                        imageURL = imageURLElm.GetString();
                        if (!Regex.IsMatch(imageURL, imageUrlPattern)) { return new ValidationDTO("Invalid Image Url"); }
                    }
                    else return new ValidationDTO("Invalid Image Url");
                }
                if (jsonElement.TryGetProperty("specification", out JsonElement specificationElm)) {
                    if (specificationElm.GetType() == typeof(string)) specification = specificationElm.GetString();
                    else return new ValidationDTO("Invalid Specification");
                }
                if (jsonElement.TryGetProperty("maintenanceIntervalDays", out JsonElement mIntervalDays)) {
                    if (mIntervalDays.TryGetInt32(out int tempMaintenanceIntervalDays)) {
                        maintenanceIntervalDays = tempMaintenanceIntervalDays;
                    } else {
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
