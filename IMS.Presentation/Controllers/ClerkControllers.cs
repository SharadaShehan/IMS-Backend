using Microsoft.AspNetCore.Mvc;
using IMS.Infrastructure.Services;
using System.Diagnostics;
using IMS.Presentation.Filters;
using IMS.ApplicationCore.DTO;
using IMS.Presentation.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using IMS.ApplicationCore.Model;
using IMS.ApplicationCore.Services;

namespace IMS.Presentation.Controllers
{
    [Route("api/clerk")]
    [ApiController]
    public class ClerkController : ControllerBase
    {
        private readonly DataBaseContext _dbContext;
        private readonly ITokenParser _tokenParser;
        private readonly IQRTokenProvider _qRTokenProvider;
        private readonly EquipmentService _equipmentService;
        private readonly ItemService _itemService;
        private readonly MaintenanceService _maintenanceService;
        private readonly ReservationService _reservationService;

        public ClerkController(DataBaseContext dbContext, ITokenParser tokenParser, IQRTokenProvider qRTokenProvider, EquipmentService equipmentService, ItemService itemService, MaintenanceService maintenanceService, ReservationService reservationService)
        {
            _dbContext = dbContext;
            _tokenParser = tokenParser;
            _qRTokenProvider = qRTokenProvider;
            _equipmentService = equipmentService;
            _itemService = itemService;
            _maintenanceService = maintenanceService;
            _reservationService = reservationService;
        }

        [HttpPost("equipments")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<EquipmentDetailedDTO>> CreateEquipment(CreateEquipmentDTO createEquipmentDTO)
        {
            try
            {
                // Validate the DTO
                if (!ModelState.IsValid) return BadRequest(ModelState);
                // Create the Equipment
                ResponseDTO<EquipmentDetailedDTO> responseDTO = _equipmentService.CreateNewEquipment(createEquipmentDTO);
                if (!responseDTO.success) return BadRequest(responseDTO.message);
                return StatusCode(201, responseDTO.result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("equipments/{id}")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<EquipmentDetailedDTO>> UpdateEquipment(int id, UpdateEquipmentDTO updateEquipmentDTO)
        {
            try
            {
                // Validate the DTO
                if (!ModelState.IsValid) return BadRequest(ModelState);
                // Update the Equipment
                ResponseDTO<EquipmentDetailedDTO> responseDTO = _equipmentService.UpdateEquipment(id, updateEquipmentDTO);
                if (!responseDTO.success) return BadRequest(responseDTO.message);
                return Ok(responseDTO.result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("equipments/{id}")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<EquipmentDTO>> DeleteEquipment(int id)
        {
            try
            {
                // Delete the Equipment
                ResponseDTO<EquipmentDetailedDTO> responseDTO = _equipmentService.DeleteEquipment(id);
                if (!responseDTO.success) return BadRequest(responseDTO.message);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("items")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<ItemDetailedDTO>> CreateItem(CreateItemDTO createItemDTO)
        {
            try
            {
                // Validate the DTO
                if (!ModelState.IsValid) return BadRequest(ModelState);
                // Create the Item
                ResponseDTO<ItemDetailedDTO> responseDTO = _itemService.CreateNewItem(createItemDTO);
                if (!responseDTO.success) return BadRequest(responseDTO.message);
                return StatusCode(201, responseDTO.result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("items/{id}")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<ItemDTO>> DeleteItem(int id)
        {
            try
            {
                // Delete the Item
                ResponseDTO<ItemDetailedDTO> responseDTO = _itemService.DeleteItem(id);
                if (!responseDTO.success) return BadRequest(responseDTO.message);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("maintenance")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<MaintenanceDetailedDTO>> CreateMaintenance(CreateMaintenanceDTO createMaintenanceDTO)
        {
            try
            {
                // Validate the DTO
                if (!ModelState.IsValid) return BadRequest(ModelState);
                // Get the User from the token
                UserDTO? clerkDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (clerkDto == null) throw new Exception("Invalid Token/Authorization Header");
                // Create the maintenance
                ResponseDTO<MaintenanceDetailedDTO> responseDTO = _maintenanceService.CreateNewMaintenance(createMaintenanceDTO, clerkDto.userId);
                if (!responseDTO.success) return BadRequest(responseDTO.message);
                return StatusCode(201, responseDTO.result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("maintenance/{id}")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<MaintenanceDetailedDTO>> ReviewMaintenance(int id, ReviewMaintenanceDTO reviewMaintenanceDTO, [FromQuery] bool accepted)
        {
            try
            {
                // Validate the DTO
                if (!ModelState.IsValid) return BadRequest(ModelState);
                // Get the User from the token
                UserDTO? clerkDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (clerkDto == null) throw new Exception("Invalid Token/Authorization Header");
                // Review the maintenance
                ResponseDTO<MaintenanceDetailedDTO> responseDTO = _maintenanceService.ReviewMaintenance(id, reviewMaintenanceDTO, clerkDto.userId, accepted);
                if (!responseDTO.success) return BadRequest(responseDTO.message);
                return Ok(responseDTO.result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("maintenance")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<List<MaintenanceDTO>>> ViewMaintenances([FromQuery] bool completed)
        {
            try
            {
                // Get the maintenances from DB
                List<MaintenanceDTO> maintenanceDTOs = await _dbContext.maintenances.Where(mnt => mnt.IsActive && (completed ? mnt.Status == "Completed" : (mnt.Status == "Ongoing" || mnt.Status == "UnderReview" || mnt.Status == "Scheduled"))).Select(mnt => new MaintenanceDTO
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

        [HttpGet("maintenance/pending")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<List<PendingMaintenanceDTO>>> ViewPendingMaintenances()
        {
            try
            {
                // Get last maintenances and filter the pending ones
                List<PendingMaintenanceDTO> maintenanceDTOs = await _dbContext.maintenances
                    .GroupBy(mnt => mnt.ItemId)
                    .Select(grp => grp.OrderByDescending(mnt => mnt.EndDate).FirstOrDefault())
                    .Where(mnt => mnt != null && mnt.IsActive && (mnt.EndDate.AddDays(mnt.Item.Equipment.MaintenanceIntervalDays ?? 10000) < DateTime.Now))
                    .Select(mnt => new PendingMaintenanceDTO
                    {
                        itemId = mnt.ItemId,
                        itemName = mnt.Item.Equipment.Name,
                        itemModel = mnt.Item.Equipment.Model,
                        itemSerialNumber = mnt.Item.SerialNumber,
                        imageUrl = mnt.Item.Equipment.ImageURL,
                        LabId = mnt.Item.Equipment.LabId,
                        LabName = mnt.Item.Equipment.Lab.LabName,
                        lastMaintenanceId = mnt.MaintenanceId,
                        lastMaintenanceStartDate = mnt.StartDate,
                        lastMaintenanceEndDate = mnt.EndDate
                    }).ToListAsync();
                return Ok(maintenanceDTOs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("reservations")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<List<ItemReservationDTO>>> ViewReservations([FromQuery] bool requested, [FromQuery] bool reserved, [FromQuery] bool borrowed)
        {
            try
            {
                // Get item reservations from DB
                List<ItemReservationDTO> reservationDTOs = await _dbContext.itemReservations.Where(rsv => rsv.IsActive && (requested ? rsv.Status == "Pending" : reserved ? rsv.Status == "Reserved" : borrowed ? rsv.Status == "Borrowed" : (rsv.Status == "Pending" || rsv.Status == "Reserved" || rsv.Status == "Borrowed"))).Select(rsv => new ItemReservationDTO
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
                    respondedAt = rsv.RespondedAt,
                    borrowedAt = rsv.BorrowedAt,
                    returnedAt = rsv.ReturnedAt,
                    cancelledAt = rsv.CancelledAt,
                    status = rsv.Status
                }).OrderByDescending(rsv => rsv.createdAt).ToListAsync();
                return Ok(reservationDTOs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("reservations/{id}")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<ItemReservationDetailedDTO>> RespondReservation([FromBody] JsonElement jsonBody, [FromQuery] bool accepted, int id)
        {
            try
            {
                // Get the User from the token
                UserDTO? clerkDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (clerkDto == null) throw new Exception("Invalid Token/Authorization Header");
                User clerk = await _dbContext.users.Where(dbUser => dbUser.IsActive && dbUser.UserId == clerkDto.userId).FirstAsync();
                // Parse the JSON
                RespondReservationDTO reservationDTO = new RespondReservationDTO(jsonBody, accepted);
                ValidationDTO validationDTO = reservationDTO.Validate();
                if (!validationDTO.success) return BadRequest(validationDTO.message);
                // Get the reservation if Available
                ItemReservation? reservation = await _dbContext.itemReservations.Where(rsv => rsv.ItemReservationId == id && rsv.IsActive && rsv.Status == "Pending").FirstAsync();
                if (reservation == null) return BadRequest("Reservation not Available for Accept/Reject");
                // If not accepted, then reject the reservation
                if (!accepted)
                {
                    reservation.Status = "Rejected";
                    reservation.RespondedClerkId = clerkDto.userId;
                    reservation.RespondedClerk = clerk;
                    reservation.ResponseNote = reservationDTO.rejectNote;
                    reservation.RespondedAt = DateTime.Now;
                    await _dbContext.SaveChangesAsync();
                    return Ok(new ItemReservationDetailedDTO
                    {
                        reservationId = reservation.ItemReservationId,
                        equipmentId = reservation.EquipmentId,
                        itemName = reservation.Equipment.Name,
                        itemModel = reservation.Equipment.Model,
                        imageUrl = reservation.Equipment.ImageURL,
                        itemId = null,
                        itemSerialNumber = null,
                        labId = reservation.Equipment.LabId,
                        labName = reservation.Equipment.Lab.LabName,
                        startDate = reservation.StartDate,
                        endDate = reservation.EndDate,
                        reservedUserId = reservation.ReservedUserId,
                        reservedUserName = reservation.ReservedUser.FirstName + " " + reservation.ReservedUser.LastName,
                        createdAt = reservation.CreatedAt,
                        respondedClerkId = reservation.RespondedClerkId,
                        respondedClerkName = reservation.RespondedClerk.FirstName + " " + reservation.RespondedClerk.LastName,
                        responseNote = reservation.ResponseNote,
                        respondedAt = reservation.RespondedAt,
                        borrowedAt = null,
                        returnedAt = null,
                        cancelledAt = null,
                        status = reservation.Status
                    });
                }
                // If accepted, then check if the item is available
                Item? item = await _dbContext.items.Where(i => i.ItemId == reservationDTO.itemId && i.IsActive).FirstOrDefaultAsync();
                if (item == null) return BadRequest("Item not Found");
                // Assign the item to the reservation
                reservation.ItemId = reservationDTO.itemId;
                reservation.Item = item;
                // Check if the item is available during the reservation period
                ItemReservation? exReservation = await _dbContext.itemReservations.Where(exrsv => exrsv.EndDate >= reservation.StartDate && exrsv.StartDate <= reservation.EndDate && exrsv.ItemId == reservation.ItemId && exrsv.IsActive && (exrsv.Status == "Reserved" || exrsv.Status == "Borrowed")).FirstAsync();
                if (exReservation != null) return BadRequest("Item not Available for Reservation");
                Maintenance? exMaintenance = await _dbContext.maintenances.Where(exmnt => exmnt.EndDate >= reservation.StartDate && exmnt.StartDate <= reservation.EndDate && exmnt.ItemId == reservation.ItemId && exmnt.IsActive && (exmnt.Status != "Completed" || exmnt.Status != "Canceled")).FirstAsync();
                if (exMaintenance != null) return BadRequest("Item not Available for Reservation");
                if (reservation.Item.Status == "Unavailable") return BadRequest("Item not Available for Reservation");
                // Accept the reservation
                reservation.RespondedClerkId = clerkDto.userId;
                reservation.RespondedClerk = clerk;
                reservation.RespondedAt = DateTime.Now;
                reservation.Status = "Reserved";
                await _dbContext.SaveChangesAsync();
                return Ok(new ItemReservationDetailedDTO
                {
                    reservationId = reservation.ItemReservationId,
                    equipmentId = reservation.EquipmentId,
                    itemName = reservation.Equipment.Name,
                    itemModel = reservation.Equipment.Model,
                    imageUrl = reservation.Equipment.ImageURL,
                    itemId = reservation.ItemId,
                    itemSerialNumber = reservation.Item.SerialNumber,
                    labId = reservation.Equipment.LabId,
                    labName = reservation.Equipment.Lab.LabName,
                    startDate = reservation.StartDate,
                    endDate = reservation.EndDate,
                    reservedUserId = reservation.ReservedUserId,
                    reservedUserName = reservation.ReservedUser.FirstName + " " + reservation.ReservedUser.LastName,
                    createdAt = reservation.CreatedAt,
                    respondedClerkId = reservation.RespondedClerkId,
                    respondedClerkName = reservation.RespondedClerk.FirstName + " " + reservation.RespondedClerk.LastName,
                    responseNote = reservation.ResponseNote,
                    respondedAt = reservation.RespondedAt,
                    borrowedAt = reservation.BorrowedAt,
                    returnedAt = reservation.ReturnedAt,
                    cancelledAt = reservation.CancelledAt,
                    status = reservation.Status
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("reservations/{id}/verify")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<QRTokenValidatedDTO>> VerifyItemBorrowing([FromQuery] string token, int id)
        {
            try
            {
                // Get the User from the token
                UserDTO? clerkDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (clerkDto == null) throw new Exception("Invalid Token/Authorization Header");
                User clerk = await _dbContext.users.Where(dbUser => dbUser.IsActive && dbUser.UserId == clerkDto.userId).FirstAsync();
                // Get the reservation if Available
                ItemReservation? reservation = await _dbContext.itemReservations.Where(rsv => rsv.ItemReservationId == id && rsv.IsActive && rsv.Status == "Reserved").FirstAsync();
                if (reservation == null) return BadRequest("Reservation not Available for Borrowing");
                // Verify the token
                DecodedQRToken decodedQRToken = await _qRTokenProvider.validateQRToken(token);
                if (!decodedQRToken.success) return BadRequest(decodedQRToken.message);
                if (decodedQRToken.eventId == null) return BadRequest("Invalid Token");
                if (decodedQRToken.isReservation != true) return BadRequest("Invalid Token");
                // Verify the reservation
                if (decodedQRToken.userId != reservation.ReservedUserId) return BadRequest("Invalid Token");
                if (decodedQRToken.eventId != reservation.ItemReservationId) return BadRequest("Invalid Token");
                // Borrow the item
                reservation.LentClerk = clerk;
                reservation.LentClerkId = clerkDto.userId;
                reservation.BorrowedAt = DateTime.Now;
                reservation.Status = "Borrowed";
                Item reservedItem = reservation.Item;
                reservedItem.Status = "Borrowed";
                await _dbContext.SaveChangesAsync();
                return Ok(new QRTokenValidatedDTO());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("reservations/{id}/token")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<QRTokenGeneratedDTO>> GetTokenForReturningItem(int id)
        {
            try
            {
                // Get the User from the token
                UserDTO? clerkDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (clerkDto == null) throw new Exception("Invalid Token/Authorization Header");
                User clerk = await _dbContext.users.Where(dbUser => dbUser.IsActive && dbUser.UserId == clerkDto.userId).FirstAsync();
                // Get the reservation if Available
                ItemReservation? reservation = await _dbContext.itemReservations.Where(rsv => rsv.ItemReservationId == id && rsv.IsActive && rsv.Status == "Borrowed").FirstAsync();
                if (reservation == null) return BadRequest("Reservation not Available for Returning");
                // Get the token
                string? token = await _qRTokenProvider.getQRToken(reservation.ItemReservationId, clerkDto.userId, true);
                if (token == null) return BadRequest("Token Generation Failed");
                // Return the token
                return Ok(new QRTokenGeneratedDTO(token));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

