using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace IMS.ApplicationCore.DTO
{
    public class ReviewMaintenanceDTO
    {
        public JsonElement jsonElement { get; set; }
        public string? reviewNote { get; set; }
        [Required]
        public bool accepted { get; set; }
        private string notePattern = @"^.{1,100}$";
        public ReviewMaintenanceDTO(JsonElement jsonElement, bool accepted)
        {
            this.jsonElement = jsonElement;
            this.accepted = accepted;
        }
        public ValidationDTO Validate()
        {
            try {
                if (!accepted) {
                    reviewNote = jsonElement.GetProperty("reviewNote").ToString();
                    if (!Regex.IsMatch(reviewNote, notePattern)) { return new ValidationDTO("Invalid Review Note"); }
                }
                return new ValidationDTO();
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                return new ValidationDTO(ex.Message);
            }
        }
    }
}
