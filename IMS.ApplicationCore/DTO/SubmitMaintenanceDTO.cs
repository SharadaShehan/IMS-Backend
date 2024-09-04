using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace IMS.ApplicationCore.DTO
{
    public class SubmitMaintenanceDTO
    {
        public JsonElement jsonElement { get; set; }
        public string? submitNote { get; set; }
        public int? cost { get; set; }
        private string notePattern = @"^.{1,100}$";
        public SubmitMaintenanceDTO(JsonElement jsonElement)
        {
            this.jsonElement = jsonElement;
        }
        public ValidationDTO Validate()
        {
            try
            {
                if (jsonElement.TryGetProperty("submitNote", out JsonElement noteElm))
                {
                    if (noteElm.GetType() == typeof(string))
                    {
                        submitNote = noteElm.GetString();
                        if (!Regex.IsMatch(submitNote, notePattern)) { return new ValidationDTO("Invalid Submit Note"); }
                    }
                    else return new ValidationDTO("Invalid Submit Note");
                }
                if (jsonElement.TryGetProperty("cost", out JsonElement costElm))
                {
                    if (costElm.TryGetInt32(out int tempCost)) cost = tempCost;
                    else return new ValidationDTO("Invalid Cost Value");
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
