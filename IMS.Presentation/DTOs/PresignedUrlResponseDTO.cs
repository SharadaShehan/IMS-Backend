namespace IMS.Presentation.DTOs
{
    public class PresignedUrlResponseDTO
    {
        public Uri presignedUrl { get; set; }
        public PresignedUrlResponseDTO(Uri presignedUrl)
        {
            this.presignedUrl = presignedUrl;
        }
    }
}
