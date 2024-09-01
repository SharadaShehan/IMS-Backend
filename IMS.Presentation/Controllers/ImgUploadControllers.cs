using Microsoft.AspNetCore.Mvc;
using IMS.Infrastructure.Services;
using IMS.Presentation.Filters;
using IMS.Presentation.DTOs;
using System.Text.Json;

namespace IMS.Presentation.Controllers
{
	[Route("api/upload-url")]
	[ApiController]
	public class ImgUploadControllers : ControllerBase
    {
		private readonly IBlobStorageClient _blobStorageClient;

		public ImgUploadControllers(IBlobStorageClient blobStorageClient)
        {
            _blobStorageClient = blobStorageClient;
        }

		[HttpPost("lab")]
		[AuthorizationFilter(["SystemAdmin"])]
        public async Task<ActionResult<PresignedUrlResponseDTO>> GetPresignedURLForLab([FromBody]JsonElement jsonBody)
		{
            try {
                // Validate the request body
                PresignedUrlRequestDTO presignedUrlRequestDTO = new PresignedUrlRequestDTO(jsonBody);
                if (!presignedUrlRequestDTO.Validate()) return BadRequest("Invalid request body");
                // Generate the presigned URL
                string blobName = "labs/" + presignedUrlRequestDTO.imageName + "." + presignedUrlRequestDTO.extension;
                TimeSpan expiryDuration = TimeSpan.FromMinutes(10);
                Uri presignedUrl = _blobStorageClient.GeneratePresignedUrl(blobName, expiryDuration);
                return Ok(new PresignedUrlResponseDTO(presignedUrl));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
		}

        [HttpPost("equipment")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<PresignedUrlResponseDTO>> GetPresignedURLForEquipment([FromBody] JsonElement jsonBody)
        {
            try {
                // Validate the request body
                PresignedUrlRequestDTO presignedUrlRequestDTO = new PresignedUrlRequestDTO(jsonBody);
                if (!presignedUrlRequestDTO.Validate()) return BadRequest("Invalid request body");
                // Generate the presigned URL
                string blobName = "equipments/" + presignedUrlRequestDTO.imageName + "." + presignedUrlRequestDTO.extension;
                TimeSpan expiryDuration = TimeSpan.FromMinutes(10);
                Uri presignedUrl = _blobStorageClient.GeneratePresignedUrl(blobName, expiryDuration);
                return Ok(new PresignedUrlResponseDTO(presignedUrl));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
