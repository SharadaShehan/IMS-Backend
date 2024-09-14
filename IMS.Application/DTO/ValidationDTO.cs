namespace IMS.Application.DTO
{
    public class ValidationDTO
    {
        public bool success { get; set; }
        public string message { get; set; }

        public ValidationDTO(string message)
        {
            this.success = false;
            this.message = message;
        }

        public ValidationDTO()
        {
            this.success = true;
            this.message = "Success";
        }
    }
}
