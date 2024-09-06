using Microsoft.AspNetCore.Mvc;
using IMS.Infrastructure.Services;
using IMS.Presentation.Filters;
using System.Text.Json;
using IMS.ApplicationCore.DTO;

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
                ValidationDTO validationDTO = presignedUrlRequestDTO.Validate();
                if (!validationDTO.success) return BadRequest(validationDTO.message);
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
                ValidationDTO validationDTO = presignedUrlRequestDTO.Validate();
                if (!validationDTO.success) return BadRequest(validationDTO.message);
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
