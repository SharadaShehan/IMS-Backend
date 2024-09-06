using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IMS.ApplicationCore.DTO
{
    public class PresignedUrlRequestDTO
    {
        public JsonElement jsonElement { get; set; }
        public string imageName { get; set; }
        public string extension { get; set; }
        private List<string> validExtensions = new List<string>(["png", "jpg", "jpeg", "webp"]);
        private string imageNamePattern = @"^.{5,20}$";
        public PresignedUrlRequestDTO(JsonElement jsonElement)
        {
            this.jsonElement = jsonElement;
        }
        public ValidationDTO Validate()
        {
            try
            {
                imageName = jsonElement.GetProperty("imageName").ToString(); if (!Regex.IsMatch(imageName, imageNamePattern)) { return new ValidationDTO("Invalid Image Name"); }
                extension = jsonElement.GetProperty("extension").ToString(); if (!validExtensions.Contains(extension)) { return new ValidationDTO("Invalid Extension"); }
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
