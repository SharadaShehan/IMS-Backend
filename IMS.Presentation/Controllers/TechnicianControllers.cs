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

        [HttpGet("maintenance")]
        [AuthorizationFilter(["Technician"])]
        public async Task<ActionResult<List<MaintenanceDTO>>> ViewMaintenances([FromQuery] bool completed)
        {
            try
            {
                // Get the User from the token
                UserDTO? technicianDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (technicianDto == null) throw new Exception("Invalid Token/Authorization Header");
                // Get the maintenances from DB
                List<MaintenanceDTO> maintenanceDTOs = await _dbContext.maintenances.Where(mnt => mnt.TechnicianId == technicianDto.UserId && (completed ? mnt.Status == "Completed" : (mnt.Status == "Ongoing" || mnt.Status == "UnderReview")) && mnt.IsActive).Select(mnt => new MaintenanceDTO
                {
                    maintenanceId = mnt.MaintenanceId,
                    itemId = mnt.ItemId,
                    startDate = mnt.StartDate,
                    endDate = mnt.EndDate,
                    createdClerkId = mnt.CreatedClerkId,
                    taskDescription = mnt.TaskDescription,
                    createdAt = mnt.CreatedAt,
                    technicianId = mnt.TechnicianId,
                    submitNote = mnt.SubmitNote,
                    submittedAt = mnt.SubmittedAt,
                    reviewedClerkId = mnt.ReviewedClerkId,
                    reviewNote = mnt.ReviewNote,
                    reviewedAt = mnt.ReviewedAt,
                    cost = mnt.Cost,
                    status = mnt.Status
                }).OrderByDescending(i => i.endDate).ToListAsync();
                return Ok(maintenanceDTOs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPatch("maintenance/{id}")]
        [AuthorizationFilter(["Technician"])]
        public async Task<ActionResult<MaintenanceDTO>> UpdateMaintenance([FromBody] JsonElement jsonBody, int id)
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
                Maintenance maintenance = await _dbContext.maintenances.Where(mnt => mnt.MaintenanceId == id && mnt.Status == "Ongoing" && mnt.IsActive).FirstAsync();
                if (maintenance == null) return BadRequest("Maintenance Not Available for Updating");
                if (maintenance.TechnicianId != technicianDto.UserId) return StatusCode(403, "Only Assigned Technician can Update Maintenance");
                maintenance.SubmitNote = maintenanceDTO.submitNote;
                maintenance.Cost = maintenanceDTO.cost;
                maintenance.Status = "UnderReview";
                maintenance.SubmittedAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();
                return Ok(maintenance);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
		}
	}
}
