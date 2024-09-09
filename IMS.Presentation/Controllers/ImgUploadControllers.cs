using Microsoft.AspNetCore.Mvc;
using IMS.Infrastructure.Services;
using IMS.Presentation.Filters;
using System.Text.Json;
using IMS.Application.DTO;

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
        public async Task<ActionResult<PresignedUrlResponseDTO>> GetPresignedURLForLab(PresignedUrlRequestDTO presignedUrlRequestDTO)
		{
            try {
                // Validate the DTO
                if (!ModelState.IsValid) return BadRequest(ModelState);
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
        public async Task<ActionResult<PresignedUrlResponseDTO>> GetPresignedURLForEquipment(PresignedUrlRequestDTO presignedUrlRequestDTO)
        {
            try {
                // Validate the DTO
                if (!ModelState.IsValid) return BadRequest(ModelState);
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
