using Microsoft.AspNetCore.Mvc;
using IMS.ApplicationCore.DTO;
using IMS.Infrastructure.Services;
using System.Diagnostics;
using IMS.Presentation.Filters;
using IMS.Presentation.Services;
using Microsoft.EntityFrameworkCore;

namespace IMS.Presentation.Controllers
{
    [Route("api/user")]
	[ApiController]
	public class UserController: ControllerBase
    {
		private readonly DataBaseContext _dbContext;
        private readonly ITokenParser _tokenParser;

		public UserController(DataBaseContext dbContext, ITokenParser tokenParser)
        {
            _dbContext = dbContext;
            _tokenParser = tokenParser;

        }

        [HttpGet("labs")]
        [AuthorizationFilter(["Clerk", "Technician", "Student", "AcademicStaff", "SystemAdmin"])]
        public async Task<ActionResult<List<LabDTO>>> GetLabsList()
        {
            try {
                return await _dbContext.labs.Where(l => l.IsActive).Select(e => new LabDTO
                {
                    labId = e.LabId,
                    labName = e.LabName,
                    labCode = e.LabCode,
                    ImageURL = e.ImageURL
                }).ToListAsync();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("equipments")]
        [AuthorizationFilter(["Clerk", "Technician", "Student", "AcademicStaff", "SystemAdmin"])]
        public async Task<ActionResult<List<EquipmentDTO>>> GetEquipmentsList([FromQuery] int labId)
        {
            try {
                if (!(labId > 0)) { return BadRequest("Lab Id not Provided"); }
                return await _dbContext.equipments.Where(e => e.IsActive && e.LabId == labId).Select(e => new EquipmentDTO
                {
                    equipmentId = e.EquipmentId,
                    name = e.Name,
                    model = e.Model,
                    labId = e.LabId,
                    imageURL = e.ImageURL,
                    specification = e.Specification,
                    maintenanceIntervalDays = e.MaintenanceIntervalDays
                }).ToListAsync();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("items")]
        [AuthorizationFilter(["Clerk", "Technician", "SystemAdmin"])]
        public async Task<ActionResult<List<ItemDTO>>> GetItemsList([FromQuery] int equipmentId)
        {
            try {
                if (!(equipmentId > 0)) { return BadRequest("Equipment Id not Provided"); }
                return await _dbContext.items.Where(i => i.IsActive && i.EquipmentId == equipmentId).Select(i => new ItemDTO
                {
                    itemId = i.ItemId,
                    equipmentId = i.EquipmentId,
                    serialNumber = i.SerialNumber,
                    status = i.Status
                }).ToListAsync();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("maintenances")]
        [AuthorizationFilter(["Clerk", "Technician", "SystemAdmin"])]
        public async Task<ActionResult<List<MaintenanceDTO>>> GetMaintenancesList([FromQuery] int itemId)
        {
            try {
                if (!(itemId > 0)) { return BadRequest("Item Id not Provided"); }
                return await _dbContext.maintenances.Where(m => m.IsActive && m.ItemId == itemId).Select(i => new MaintenanceDTO
                {
                    maintenanceId = i.MaintenanceId,
                    itemId = i.ItemId,
                    startDate = i.StartDate,
                    endDate = i.EndDate,
                    createdClerkId = i.CreatedClerkId,
                    taskDescription = i.TaskDescription,
                    createdAt = i.CreatedAt,
                    technicianId = i.TechnicianId,
                    submitNote = i.SubmitNote,
                    submittedAt = i.SubmittedAt,
                    reviewedClerkId = i.ReviewedClerkId,
                    reviewNote = i.ReviewNote,
                    reviewedAt = i.ReviewedAt,
                    cost = i.Cost,
                    status = i.Status
                }).ToListAsync();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("reservations")]
        [AuthorizationFilter(["Clerk", "Technician", "SystemAdmin"])]
        public async Task<ActionResult<List<ItemReservationDTO>>> GetReservationsList([FromQuery] int itemId)
        {
            try {
                if (!(itemId > 0)) { return BadRequest("Item Id not Provided"); }
                return await _dbContext.itemReservations.Where(r => r.ItemId == itemId).Select(r => new ItemReservationDTO
                {
                    itemReservationId = r.ItemReservationId,
                    equipmentId = r.EquipmentId,
                    itemId = r.ItemId,
                    startDate = r.StartDate,
                    endDate = r.EndDate,
                    reservedUserId = r.ReservedUserId,
                    createdAt = r.CreatedAt,
                    respondedClerkId = r.RespondedClerkId,
                    responseNote = r.ResponseNote,
                    respondedAt = r.RespondedAt,
                    lentClerkId = r.LentClerkId,
                    borrowedAt = r.BorrowedAt,
                    returnAcceptedClerkId = r.ReturnAcceptedClerkId,
                    returnedAt = r.ReturnedAt,
                    cancelledAt = r.CancelledAt,
                    status = r.Status
                }).ToListAsync();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("role")]
		[AuthorizationFilter(["Clerk", "Technician", "Student", "AcademicStaff", "SystemAdmin"])]
        public async Task<ActionResult<UserRoleDTO>> GetUserRole()
		{
            try {
                // Get the User from the token
                UserDTO? user = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (user == null) throw new Exception("Invalid Token/Authorization Header");
                // Return the user's role
                return Ok(new UserRoleDTO(user.Role));
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
		}


	}
}
