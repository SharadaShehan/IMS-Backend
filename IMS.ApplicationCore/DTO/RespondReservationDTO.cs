using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace IMS.ApplicationCore.DTO
{
    public class RespondReservationDTO
    {
        public JsonElement jsonElement { get; set; }
        public int? itemId { get; set; }
        public string? rejectNote { get; set; }
        [Required]
        public bool accepted { get; set; }
        private string notePattern = @"^.{1,100}$";
        public RespondReservationDTO(JsonElement jsonElement, bool accepted)
        {
            this.jsonElement = jsonElement;
            this.accepted = accepted;
        }
        public ValidationDTO Validate()
        {
            try {
                if (!accepted) {
                    rejectNote = jsonElement.GetProperty("rejectNote").ToString();
                    if (!Regex.IsMatch(rejectNote, notePattern)) { return new ValidationDTO("Invalid Reject Note"); }
                }
                if (accepted)
                {
                    if (jsonElement.TryGetProperty("itemId", out JsonElement itemIdElem))
                    {
                        if (itemIdElem.TryGetInt32(out int tempItemId))
                        {
                            itemId = tempItemId;
                        }
                        else
                        {
                            return new ValidationDTO("Invalid Item Id");
                        }
                    }
                }
                return new ValidationDTO();
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                return new ValidationDTO(ex.Message);
            }
        }
    }
}
