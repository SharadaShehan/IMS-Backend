using Microsoft.AspNetCore.Mvc;
using IMS.ApplicationCore.DTO;
using IMS.Infrastructure.Services;
using System.Diagnostics;
using IMS.Presentation.Filters;
using IMS.Presentation.Services;
using Microsoft.EntityFrameworkCore;
using IMS.ApplicationCore.Model;

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
                    imageUrl = e.ImageURL
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
                if (!(labId > 0)) { return BadRequest("Invalid Lab Id"); }
                return await _dbContext.equipments.Where(e => e.IsActive && e.LabId == labId).Select(e => new EquipmentDTO
                {
                    equipmentId = e.EquipmentId,
                    name = e.Name,
                    model = e.Model,
                    imageUrl = e.ImageURL,
                    labId = e.LabId,
                    labName = e.Lab.LabName,
                    specification = e.Specification,
                    maintenanceIntervalDays = e.MaintenanceIntervalDays
                }).ToListAsync();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("equipments/{id}")]
        [AuthorizationFilter(["Clerk", "Technician", "Student", "AcademicStaff", "SystemAdmin"])]
        public async Task<ActionResult<EquipmentDetailedDTO>> GetDetailedEquipment(int id)
        {
            try
            {
                if (!(id > 0)) { return BadRequest("Invalid Equipment Id"); }
                int totalCount = await _dbContext.items.Where(i => i.EquipmentId == id && i.IsActive).CountAsync();
                int reservedCount = await _dbContext.itemReservations.Where(ir => ir.EquipmentId == id && ir.Status == "Reserved" && ir.IsActive).CountAsync();
                // availableCount = number of items currently physically available in the lab
                int availableCount = await _dbContext.items.Where(i => i.EquipmentId == id && i.IsActive && i.Status == "Available").CountAsync();
                return await _dbContext.equipments.Where(e => e.IsActive && e.EquipmentId == id).Select(e => new EquipmentDetailedDTO
                {
                    equipmentId = e.EquipmentId,
                    name = e.Name,
                    model = e.Model,
                    imageUrl = e.ImageURL,
                    labId = e.LabId,
                    labName = e.Lab.LabName,
                    specification = e.Specification,
                    maintenanceIntervalDays = e.MaintenanceIntervalDays,
                    totalCount = totalCount,
                    reservedCount = reservedCount,
                    availableCount = availableCount
                }).FirstAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("items")]
        [AuthorizationFilter(["Clerk", "Technician", "SystemAdmin"])]
        public async Task<ActionResult<List<ItemDTO>>> GetItemsList([FromQuery] int equipmentId)
        {
            try {
                if (!(equipmentId > 0)) { return BadRequest("Invalid Equipment Id"); }
                return await _dbContext.items.Where(i => i.IsActive && i.EquipmentId == equipmentId).Select(i => new ItemDTO
                {
                    itemId = i.ItemId,
                    imageUrl = i.Equipment.ImageURL,
                    equipmentId = i.EquipmentId,
                    serialNumber = i.SerialNumber,
                    status = i.Status
                }).ToListAsync();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("items/{id}")]
        [AuthorizationFilter(["Clerk", "Technician", "SystemAdmin"])]
        public async Task<ActionResult<ItemDetailedDTO>> GetDetailedItem(int id)
        {
            try
            {
                if (!(id > 0)) { return BadRequest("Invalid Item Id"); }
                Maintenance? lastMaintenance = await _dbContext.maintenances.Where(m => m.ItemId == id && m.Status != "Canceled" && m.IsActive).OrderByDescending(m => m.EndDate).FirstOrDefaultAsync();
                return await _dbContext.items.Where(i => i.IsActive && i.ItemId == id).Select(i => new ItemDetailedDTO
                {
                    itemId = i.ItemId,
                    imageUrl = i.Equipment.ImageURL,
                    equipmentId = i.EquipmentId,
                    serialNumber = i.SerialNumber,
                    status = i.Status,
                    itemName = i.Equipment.Name,
                    itemModel = i.Equipment.Model,
                    labId = i.Equipment.LabId,
                    labName = i.Equipment.Lab.LabName,
                    lastMaintenanceOn = lastMaintenance != null ? lastMaintenance.EndDate : null,
                    lastMaintenanceBy = lastMaintenance != null ? lastMaintenance.Technician.FirstName + " " + lastMaintenance.Technician.LastName : null
                }).FirstAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("maintenances")]
        [AuthorizationFilter(["Clerk", "Technician", "SystemAdmin"])]
        public async Task<ActionResult<List<MaintenanceDetailedDTO>>> GetMaintenancesList([FromQuery] int itemId)
        {
            try {
                if (!(itemId > 0)) { return BadRequest("Invalid Item Id"); }
                return await _dbContext.maintenances.Where(m => m.IsActive && m.ItemId == itemId).Select(i => new MaintenanceDetailedDTO
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
                }).OrderByDescending(i => i.endDate).ToListAsync();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("maintenance/{id}")]
        [AuthorizationFilter(["Clerk", "Technician", "SystemAdmin"])]
        public async Task<ActionResult<MaintenanceDetailedDTO>> ViewDetailedMaintenance(int id)
        {
            try
            {
                // Get the maintenances from DB
                MaintenanceDetailedDTO maintenanceDTO = await _dbContext.maintenances.Where(mnt => mnt.IsActive && mnt.MaintenanceId == id).Select(mnt => new MaintenanceDetailedDTO
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
                    createdClerkId = mnt.CreatedClerkId,
                    createdClerkName = mnt.CreatedClerk.FirstName + " " + mnt.CreatedClerk.LastName,
                    taskDescription = mnt.TaskDescription,
                    createdAt = mnt.CreatedAt,
                    technicianId = mnt.TechnicianId,
                    technicianName = mnt.Technician.FirstName + " " + mnt.Technician.LastName,
                    submitNote = mnt.SubmitNote,
                    submittedAt = mnt.SubmittedAt,
                    reviewedClerkId = mnt.ReviewedClerkId,
                    reviewedClerkName = mnt.ReviewedClerk != null ? mnt.ReviewedClerk.FirstName + " " + mnt.ReviewedClerk.LastName : null,
                    reviewNote = mnt.ReviewNote,
                    reviewedAt = mnt.ReviewedAt,
                    cost = mnt.Cost,
                    status = mnt.Status
                }).FirstAsync();
                return Ok(maintenanceDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("reservations")]
        [AuthorizationFilter(["Clerk", "Technician", "SystemAdmin"])]
        public async Task<ActionResult<List<ItemReservationDTO>>> GetReservationsList([FromQuery] int itemId)
        {
            try {
                if (!(itemId > 0)) { return BadRequest("Invalid Item Id"); }
                return await _dbContext.itemReservations.Where(r => r.ItemId == itemId && r.IsActive).Select(r => new ItemReservationDTO
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
                }).OrderByDescending(i => i.startDate).ToListAsync();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("reservations/{id}")]
        [AuthorizationFilter(["Clerk", "Technician", "SystemAdmin"])]
        public async Task<ActionResult<ItemReservationDetailedDTO>> ViewDetailedReservation(int id)
        {
            try
            {
                // Get the reservations from DB
                ItemReservationDetailedDTO reservationDTO = await _dbContext.itemReservations.Where(rsv => rsv.IsActive && rsv.ItemReservationId == id).Select(rsv => new ItemReservationDetailedDTO
                {
                    reservationId = rsv.ItemReservationId,
                    equipmentId = rsv.EquipmentId,
                    itemName = rsv.Equipment.Name,
                    itemModel = rsv.Equipment.Model,
                    imageUrl = rsv.Equipment.ImageURL,
                    itemId = rsv.ItemId,
                    itemSerialNumber = rsv.Item != null ? rsv.Item.SerialNumber : null,
                    labId = rsv.Equipment.LabId,
                    labName = rsv.Equipment.Lab.LabName,
                    startDate = rsv.StartDate,
                    endDate = rsv.EndDate,
                    reservedUserId = rsv.ReservedUserId,
                    reservedUserName = rsv.ReservedUser.FirstName + " " + rsv.ReservedUser.LastName,
                    createdAt = rsv.CreatedAt,
                    respondedClerkId = rsv.RespondedClerkId,
                    respondedClerkName = rsv.RespondedClerk != null ? rsv.RespondedClerk.FirstName + " " + rsv.RespondedClerk.LastName : null,
                    responseNote = rsv.ResponseNote,
                    respondedAt = rsv.RespondedAt,
                    lentClerkId = rsv.LentClerkId,
                    lentClerkName = rsv.LentClerk != null ? rsv.LentClerk.FirstName + " " + rsv.LentClerk.LastName : null,
                    borrowedAt = rsv.BorrowedAt,
                    returnAcceptedClerkId = rsv.ReturnAcceptedClerkId,
                    returnAcceptedClerkName = rsv.ReturnAcceptedClerk != null ? rsv.ReturnAcceptedClerk.FirstName + " " + rsv.ReturnAcceptedClerk.LastName : null,
                    returnedAt = rsv.ReturnedAt,
                    cancelledAt = rsv.CancelledAt,
                    status = rsv.Status
                }).FirstAsync();
                return Ok(reservationDTO);
            }
            catch (Exception ex)
            {
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
