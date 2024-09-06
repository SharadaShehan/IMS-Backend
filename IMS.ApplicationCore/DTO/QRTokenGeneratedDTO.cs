namespace IMS.ApplicationCore.DTO
{
    public class QRTokenGeneratedDTO
    {
        public string qrToken { get; set; }
        public QRTokenGeneratedDTO(string qrToken)
        {
            this.qrToken = qrToken;
        }
    }
}
