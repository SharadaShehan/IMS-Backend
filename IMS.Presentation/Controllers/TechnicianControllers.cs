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
                List<MaintenanceDTO> maintenanceDTOs = await _dbContext.maintenances.Where(mnt => mnt.TechnicianId == technicianDto.UserId && (completed ? mnt.Status == "Completed" : (mnt.Status == "Ongoing" || mnt.Status == "UnderReview" || mnt.Status == "Scheduled")) && mnt.IsActive).Select(mnt => new MaintenanceDTO
                {
                    maintenanceId = mnt.MaintenanceId,
                    itemId = mnt.ItemId,
                    itemName = mnt.Item.Equipment.Name,
                    itemModel = mnt.Item.Equipment.Model,
                    imageUrl = mnt.Item.Equipment.ImageURL,
                    itemSerialNumber = mnt.Item.SerialNumber,
                    labId = mnt.Item.Equipment.LabId,
                    labName = mnt.Item.Equipment.Lab.LabName,
                    startDate = mnt.StartDate,
                    endDate = mnt.EndDate,
                    createdAt = mnt.CreatedAt,
                    submittedAt = mnt.SubmittedAt,
                    reviewedAt = mnt.ReviewedAt,
                    status = mnt.Status
                }).OrderByDescending(i => i.endDate).ToListAsync();
                return Ok(maintenanceDTOs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("maintenance/{id}/borrow")]
        [AuthorizationFilter(["Technician"])]
        public async Task<ActionResult<MaintenanceDTO>> BorrowItem(int id)
        {
            try
            {
                // Get the User from the token
                UserDTO? technicianDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (technicianDto == null) throw new Exception("Invalid Token/Authorization Header");
                // Get the maintenance from DB
                Maintenance maintenance = await _dbContext.maintenances.Where(mnt => mnt.MaintenanceId == id && mnt.Status == "Scheduled" && mnt.IsActive).FirstAsync();
                if (maintenance == null) return BadRequest("Maintenance Not Available");
                if (maintenance.TechnicianId != technicianDto.UserId) return StatusCode(403, "Only Assigned Technician can Borrow Item");
                // Get the item from DB
                Item item = await _dbContext.items.Where(itm => itm.ItemId == maintenance.ItemId && itm.Status == "Available" && itm.IsActive).FirstAsync();
                if (item == null) return BadRequest("Item Not Available for Borrowing");
                item.Status = "UnderRepair";
                maintenance.Status = "Ongoing";
                await _dbContext.SaveChangesAsync();
                return Ok(new MaintenanceDTO
                {
                    maintenanceId = maintenance.MaintenanceId,
                    itemId = maintenance.ItemId,
                    itemName = maintenance.Item.Equipment.Name,
                    itemModel = maintenance.Item.Equipment.Model,
                    imageUrl = maintenance.Item.Equipment.ImageURL,
                    itemSerialNumber = maintenance.Item.SerialNumber,
                    labId = maintenance.Item.Equipment.LabId,
                    labName = maintenance.Item.Equipment.Lab.LabName,
                    startDate = maintenance.StartDate,
                    endDate = maintenance.EndDate,
                    createdAt = maintenance.CreatedAt,
                    submittedAt = maintenance.SubmittedAt,
                    reviewedAt = maintenance.ReviewedAt,
                    status = maintenance.Status
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPatch("maintenance/{id}")]
        [AuthorizationFilter(["Technician"])]
        public async Task<ActionResult<MaintenanceDetailedDTO>> UpdateMaintenance([FromBody] JsonElement jsonBody, int id)
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
                return Ok(new MaintenanceDetailedDTO
                {
                    maintenanceId = maintenance.MaintenanceId,
                    itemId = maintenance.ItemId,
                    itemName = maintenance.Item.Equipment.Name,
                    itemModel = maintenance.Item.Equipment.Model,
                    imageUrl = maintenance.Item.Equipment.ImageURL,
                    itemSerialNumber = maintenance.Item.SerialNumber,
                    labId = maintenance.Item.Equipment.LabId,
                    labName = maintenance.Item.Equipment.Lab.LabName,
                    startDate = maintenance.StartDate,
                    endDate = maintenance.EndDate,
                    createdClerkId = maintenance.CreatedClerkId,
                    createdClerkName = maintenance.CreatedClerk.FirstName + " " + maintenance.CreatedClerk.LastName,
                    taskDescription = maintenance.TaskDescription,
                    createdAt = maintenance.CreatedAt,
                    technicianId = maintenance.TechnicianId,
                    technicianName = maintenance.Technician.FirstName + " " + maintenance.Technician.LastName,
                    submitNote = maintenance.SubmitNote,
                    submittedAt = maintenance.SubmittedAt,
                    reviewedClerkId = maintenance.ReviewedClerkId,
                    reviewedClerkName = maintenance.ReviewedClerk.FirstName + " " + maintenance.ReviewedClerk.LastName,
                    reviewNote = maintenance.ReviewNote,
                    reviewedAt = maintenance.ReviewedAt,
                    cost = maintenance.Cost,
                    status = maintenance.Status
                });
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
		}
	}
}
