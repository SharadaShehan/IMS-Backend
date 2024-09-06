using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IMS.ApplicationCore.DTO
{
    public class CreateLabDTO
    {
        public JsonElement jsonElement { get; set; }
        [Required]
        public string labName { get; set; }
        [Required]
        public string labCode { get; set; }
        public string? imageURL { get; set; }
        private string textPattern = @"^.{2,20}$";
        private string imageUrlPattern = @"^https?:\/\/.*\.(?:png|jpg|jpeg|webp)$";
        public CreateLabDTO(JsonElement jsonElement)
        {
            this.jsonElement = jsonElement;
        }
        public ValidationDTO Validate()
        {
            try
            {
                labName = jsonElement.GetProperty("labName").ToString(); if (!Regex.IsMatch(labName, textPattern)) { return new ValidationDTO("Invalid Lab Name"); }
                labCode = jsonElement.GetProperty("labCode").ToString(); if (!Regex.IsMatch(labCode, textPattern)) { return new ValidationDTO("Invalid Lab Code"); }
                if (jsonElement.TryGetProperty("imageURL", out JsonElement imageURLElm)) {
                    if (imageURLElm.GetType() == typeof(string))
                    {
                        imageURL = imageURLElm.GetString();
                        if (!Regex.IsMatch(imageURL, imageUrlPattern)) { return new ValidationDTO("Invalid Image Url"); }
                    } else {
                        return new ValidationDTO("Invalid Image Url");
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
