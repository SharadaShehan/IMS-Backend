namespace IMS.Application.DTO
{
    public class ResponseDTO<T>
    {
        public bool success { get; set; }
        public T? result { get; set; }
        public string? message { get; set; }

        public ResponseDTO(T obj)
        {
            success = true;
            result = obj;
        }

        public ResponseDTO(string message)
        {
            success = false;
            this.message = message;
        }
    }
}
