namespace IMS.ApplicationCore.DTO
{
    public class QRTokenValidatedDTO
    {
        public bool success { get; set; }
        public QRTokenValidatedDTO()
        {
            this.success = true;
        }
    }
}
