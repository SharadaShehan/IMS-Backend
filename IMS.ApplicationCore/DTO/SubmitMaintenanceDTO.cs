using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IMS.ApplicationCore.DTO
{
    public class SubmitMaintenanceDTO
    {
        public JsonElement jsonElement { get; set; }
        public string submitNote { get; set; }
        private string notePattern = @"^.{1,100}$";
        public SubmitMaintenanceDTO(JsonElement jsonElement)
        {
            this.jsonElement = jsonElement;
        }
        public ValidationDTO Validate()
        {
            try
            {
                submitNote = jsonElement.GetProperty("submitNote").ToString(); if (!Regex.IsMatch(submitNote, notePattern)) { return new ValidationDTO("Invalid Submit Note"); }
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
