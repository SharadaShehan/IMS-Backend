using Microsoft.AspNetCore.Mvc;
using IMS.Infrastructure.Services;
using System.Diagnostics;
using IMS.Presentation.Filters;
using IMS.ApplicationCore.DTO;
using IMS.Presentation.Services;
using Microsoft.EntityFrameworkCore;
using IMS.ApplicationCore.Model;
using System.Text.Json;

namespace IMS.Presentation.Controllers
{
	[Route("api/technician")]
    [ApiController]
	public class TechnicianController : ControllerBase
    {
		private readonly DataBaseContext _dbContext;
        private readonly ITokenParser _tokenParser;

		public TechnicianController(DataBaseContext dbContext, ITokenParser tokenParser)
        {
            _dbContext = dbContext;
            _tokenParser = tokenParser;
        }

		[HttpPatch("maintenance/{id}")]
        [AuthorizationFilter(["Technician"])]
        public async Task<ActionResult<Maintenance>> UpdateMaintenance([FromBody] JsonElement jsonBody, int id)
        {
            try {
                // Get the User from the token
                UserDTO? technicianDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (technicianDto == null) throw new Exception("Invalid Token/Authorization Header");
                // Parse the JSON
                SubmitMaintenanceDTO maintenanceDTO = new SubmitMaintenanceDTO(jsonBody);
                ValidationDTO validationDTO = maintenanceDTO.Validate();
                if (!validationDTO.success) return BadRequest(validationDTO.message);
                // Get the maintenance from DB
                Maintenance maintenance = await _dbContext.maintenances.Where(mnt => mnt.MaintenanceId == id && mnt.TechnicianId == technicianDto.UserId && mnt.Status == "Ongoing" && mnt.IsActive).FirstAsync();
                if (maintenance == null) return BadRequest("Maintenance Not Available for Updating");
                maintenance.SubmitNote = maintenanceDTO.submitNote;
                maintenance.SubmittedAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();
                return Ok(maintenance);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
		}
	}
}
