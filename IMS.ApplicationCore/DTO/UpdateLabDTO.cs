using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IMS.ApplicationCore.DTO
{
    public class UpdateLabDTO
    {
        public JsonElement jsonElement { get; set; }
        public string? labName { get; set; }
        public string? labCode { get; set; }
        public string? imageURL { get; set; }
        private string textPattern = @"^.{2,20}$";
        private string imageUrlPattern = @"^https?:\/\/.*\.(?:png|jpg|jpeg|webp)$";
        public UpdateLabDTO(JsonElement jsonElement)
        {
            this.jsonElement = jsonElement;
        }
        public ValidationDTO Validate()
        {
            try
            {
                if (jsonElement.TryGetProperty("labName", out JsonElement nameElm)) {
                    if (nameElm.GetType() == typeof(string))
                    {
                        labName = nameElm.GetString();
                        if (!Regex.IsMatch(labName, textPattern)) { return new ValidationDTO("Invalid Lab Name"); }
                    }
                    else return new ValidationDTO("Invalid Lab Name");
                }
                if (jsonElement.TryGetProperty("labCode", out JsonElement modelElm)) {
                    if (modelElm.GetType() == typeof(string))
                    {
                        labCode = modelElm.GetString();
                        if (!Regex.IsMatch(labCode, textPattern)) { return new ValidationDTO("Invalid Lab Model"); }
                    }
                    else return new ValidationDTO("Invalid Lab Model");
                }
                if (jsonElement.TryGetProperty("imageURL", out JsonElement imageURLElm)) {
                    if (imageURLElm.GetType() == typeof(string))
                    {
                        imageURL = imageURLElm.GetString();
                        if (!Regex.IsMatch(imageURL, imageUrlPattern)) { return new ValidationDTO("Invalid Image Url"); }
                    }
                    else return new ValidationDTO("Invalid Image Url");
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
