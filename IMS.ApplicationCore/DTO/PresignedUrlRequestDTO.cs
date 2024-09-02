using System.Diagnostics;
using System.Text.Json;

namespace IMS.ApplicationCore.DTO
{
    public class PresignedUrlRequestDTO
    {
        public JsonElement jsonElement { get; set; }
        public string imageName { get; set; }
        public string extension { get; set; }
        private List<string> validExtensions = new List<string>(["png", "jpg", "jpeg", "webp"]);
        public PresignedUrlRequestDTO(JsonElement jsonElement)
        {
            this.jsonElement = jsonElement;
        }
        public bool Validate()
        {
            try
            {
                imageName = jsonElement.GetProperty("imageName").ToString();
                extension = jsonElement.GetProperty("extension").ToString();
                if (imageName == null || extension == null) return false;
                if (imageName.Length > 20 || imageName.Length < 5) return false;
                if (!validExtensions.Contains(extension)) return false;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
