﻿namespace IMS.ApplicationCore.DTO
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
