using Microsoft.AspNetCore.Mvc;
using IMS.Infrastructure.Services;
using System.Diagnostics;
using IMS.Presentation.Filters;
using IMS.ApplicationCore.DTO;
using IMS.Presentation.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using IMS.ApplicationCore.Model;

namespace IMS.Presentation.Controllers
{
    [Route("api/clerk")]
    [ApiController]
    public class ClerkController : ControllerBase
    {
        private readonly DataBaseContext _dbContext;
        private readonly ITokenParser _tokenParser;
        private readonly IQRTokenProvider _qRTokenProvider;

        public ClerkController(DataBaseContext dbContext, ITokenParser tokenParser, IQRTokenProvider qRTokenProvider)
        {
            _dbContext = dbContext;
            _tokenParser = tokenParser;
            _qRTokenProvider = qRTokenProvider;
        }


        [HttpPost("equipments")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<EquipmentDetailedDTO>> CreateEquipment([FromBody] JsonElement jsonBody)
        {
            try
            {
                // Parse the JSON
                CreateEquipmentDTO equipmentDTO = new CreateEquipmentDTO(jsonBody);
                ValidationDTO validationDTO = equipmentDTO.Validate();
                if (!validationDTO.success) return BadRequest(validationDTO.message);
                // Get the Lab
                Lab? lab = await _dbContext.labs.Where(l => l.LabId == equipmentDTO.labId && l.IsActive).FirstAsync();
                if (lab == null) return BadRequest("Lab not Found");
                // Prevent Duplicate Equipment Creation
                Equipment? existingEquipment = await _dbContext.equipments.Where(e => e.Name == equipmentDTO.name && e.Model == equipmentDTO.model && e.LabId == equipmentDTO.labId && e.IsActive).FirstOrDefaultAsync();
                if (existingEquipment != null) return BadRequest("Equipment Already Exists");
                // Create the Equipment
                Equipment newEquipment = new Equipment
                {
                    Name = equipmentDTO.name,
                    Model = equipmentDTO.model,
                    LabId = equipmentDTO.labId,
                    Lab = lab,
                    ImageURL = equipmentDTO.imageURL,
                    Specification = equipmentDTO.specification,
                    MaintenanceIntervalDays = equipmentDTO.maintenanceIntervalDays,
                    IsActive = true
                };
                await _dbContext.equipments.AddAsync(newEquipment);
                await _dbContext.SaveChangesAsync();
                return StatusCode(201, new EquipmentDetailedDTO
                {
                    equipmentId = newEquipment.EquipmentId,
                    name = newEquipment.Name,
                    model = newEquipment.Model,
                    imageUrl = newEquipment.ImageURL,
                    labId = newEquipment.LabId,
                    labName = newEquipment.Lab.LabName,
                    specification = newEquipment.Specification,
                    maintenanceIntervalDays = newEquipment.MaintenanceIntervalDays,
                    totalCount = 0,
                    reservedCount = 0,
                    availableCount = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPatch("equipments/{id}")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<EquipmentDetailedDTO>> UpdateEquipment([FromBody] JsonElement jsonBody, int id)
        {
            try
            {
                // Parse the JSON
                UpdateEquipmentDTO equipmentDTO = new UpdateEquipmentDTO(jsonBody);
                ValidationDTO validationDTO = equipmentDTO.Validate();
                if (!validationDTO.success) return BadRequest(validationDTO.message);
                // Get the Equipment to be Updated
                Equipment? equipment = await _dbContext.equipments.Where(e => e.EquipmentId == id && e.IsActive).FirstOrDefaultAsync();
                if (equipment == null) return BadRequest("Equipment Not Found");
                // Update the Equipment
                if (equipmentDTO.name != null) equipment.Name = equipmentDTO.name;
                if (equipmentDTO.model != null) equipment.Model = equipmentDTO.model;
                if (equipmentDTO.imageURL != null) equipment.ImageURL = equipmentDTO.imageURL;
                if (equipmentDTO.specification != null) equipment.Specification = equipmentDTO.specification;
                if (equipmentDTO.maintenanceIntervalDays != null) equipment.MaintenanceIntervalDays = equipmentDTO.maintenanceIntervalDays;
                await _dbContext.SaveChangesAsync();
                int totalCount = await _dbContext.items.Where(i => i.EquipmentId == id && i.IsActive).CountAsync();
                int reservedCount = await _dbContext.itemReservations.Where(ir => ir.EquipmentId == id && ir.Status == "Reserved" && ir.IsActive).CountAsync();
                // availableCount = number of items currently physically available in the lab
                int availableCount = await _dbContext.items.Where(i => i.EquipmentId == id && i.IsActive && i.Status == "Available").CountAsync();
                return Ok(new EquipmentDetailedDTO
                {
                    equipmentId = equipment.EquipmentId,
                    name = equipment.Name,
                    model = equipment.Model,
                    labId = equipment.LabId,
                    labName = equipment.Lab.LabName,
                    imageUrl = equipment.ImageURL,
                    specification = equipment.Specification,
                    maintenanceIntervalDays = equipment.MaintenanceIntervalDays,
                    totalCount = totalCount,
                    reservedCount = reservedCount,
                    availableCount = availableCount
                });
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
                Equipment? equipment = await _dbContext.equipments.Where(e => e.EquipmentId == id && e.IsActive).FirstOrDefaultAsync();
                if (equipment == null) return BadRequest("Equipment Not Found");
                equipment.IsActive = false;
                // Delete all items of the equipment
                List<Item> items = await _dbContext.items.Where(i => i.EquipmentId == id).ToListAsync();
                foreach (Item item in items)
                {
                    item.IsActive = false;
                }
                // Delete all maintenance of items of the equipment
                List<Maintenance> maintenances = await _dbContext.maintenances.Where(m => m.Item.EquipmentId == id).ToListAsync();
                foreach (Maintenance maintenance in maintenances)
                {
                    maintenance.IsActive = false;
                }
                // Delete all reservations of items of the equipment
                List<ItemReservation> reservations = await _dbContext.itemReservations.Where(ir => ir.Item != null && ir.Item.EquipmentId == id).ToListAsync();
                foreach (ItemReservation reservation in reservations)
                {
                    reservation.IsActive = false;
                }
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("items")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<ItemDetailedDTO>> CreateItem([FromBody] JsonElement jsonBody)
        {
            try
            {
                // Parse the JSON 
                CreateItemDTO itemDTO = new CreateItemDTO(jsonBody);
                ValidationDTO validationDTO = itemDTO.Validate();
                if (!validationDTO.success) return BadRequest(validationDTO.message);
                // Get the Equipment
                Equipment? equipment = await _dbContext.equipments.Where(e => e.EquipmentId == itemDTO.equipmentId && e.IsActive).FirstAsync();
                if (equipment == null) return BadRequest("Equipment not Found");
                // Prevent Duplicate Item Creation
                Item? existingItem = await _dbContext.items.Where(i => i.EquipmentId == itemDTO.equipmentId && i.SerialNumber == itemDTO.serialNumber && i.IsActive).FirstOrDefaultAsync();
                if (existingItem != null) return BadRequest("Item Already Exists");
                // Create the Equipment
                Item newItem = new Item
                {
                    EquipmentId = itemDTO.equipmentId,
                    Equipment = equipment,
                    SerialNumber = itemDTO.serialNumber,
                    Status = "Available",
                    IsActive = true
                };
                await _dbContext.items.AddAsync(newItem);
                await _dbContext.SaveChangesAsync();
                return StatusCode(201, new ItemDetailedDTO
                {
                    itemId = newItem.ItemId,
                    itemName = newItem.Equipment.Name,
                    itemModel = newItem.Equipment.Model,
                    imageUrl = newItem.Equipment.ImageURL,
                    equipmentId = newItem.EquipmentId,
                    labId = newItem.Equipment.Lab.LabId,
                    labName = newItem.Equipment.Lab.LabName,
                    serialNumber = newItem.SerialNumber,
                    status = newItem.Status
                });
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
                Item? item = await _dbContext.items.Where(i => i.ItemId == id && i.IsActive).FirstOrDefaultAsync();
                if (item == null) return BadRequest("Item Not Found");
                item.IsActive = false;
                // Delete all maintenance of the item
                List<Maintenance> maintenances = await _dbContext.maintenances.Where(m => m.ItemId == id).ToListAsync();
                foreach (Maintenance maintenance in maintenances)
                {
                    maintenance.IsActive = false;
                }
                // Delete all reservations of the item
                List<ItemReservation> reservations = await _dbContext.itemReservations.Where(ir => ir.ItemId == id).ToListAsync();
                foreach (ItemReservation reservation in reservations)
                {
                    reservation.IsActive = false;
                }
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("maintenance")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<MaintenanceDetailedDTO>> CreateMaintenance([FromBody] JsonElement jsonBody)
        {
            try
            {
                // Get the User from the token
                UserDTO? clerkDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (clerkDto == null) throw new Exception("Invalid Token/Authorization Header");
                User clerk = await _dbContext.users.Where(dbUser => dbUser.IsActive && dbUser.UserId == clerkDto.UserId).FirstAsync();
                // Parse the JSON
                CreateMaintenanceDTO maintenanceDTO = new CreateMaintenanceDTO(jsonBody);
                ValidationDTO validationDTO = maintenanceDTO.Validate();
                if (!validationDTO.success) return BadRequest(validationDTO.message);
                // Check if the item will be available during the maintenance period
                ItemReservation? exReservation = await _dbContext.itemReservations.Where(exrsv => exrsv.EndDate >= maintenanceDTO.startDate && exrsv.StartDate <= maintenanceDTO.endDate && exrsv.ItemId == maintenanceDTO.itemId && exrsv.IsActive && (exrsv.Status == "Reserved" || exrsv.Status == "Borrowed")).FirstAsync();
                if (exReservation != null) { return BadRequest("Item not Available for Maintenance"); }
                Maintenance? exMaintenance = await _dbContext.maintenances.Where(exmnt => exmnt.EndDate >= maintenanceDTO.startDate && exmnt.StartDate <= maintenanceDTO.endDate && exmnt.ItemId == maintenanceDTO.itemId && exmnt.IsActive && (exmnt.Status != "Completed" || exmnt.Status != "Canceled")).FirstAsync();
                if (exMaintenance != null) { return BadRequest("Item not Available for Maintenance"); }
                // Get the item if Available
                Item? item = await _dbContext.items.Where(it => it.ItemId == maintenanceDTO.itemId && it.IsActive && it.Status != "Unavailable").FirstAsync();
                if (item == null) return BadRequest("Item not Available for Maintenance");
                // Get the technician
                User? technician = await _dbContext.users.Where(u => u.UserId == maintenanceDTO.technicianId && u.IsActive && u.Role == "Technician").FirstAsync();
                if (technician == null) return BadRequest("Technician not Found");
                // Create the maintenance
                Maintenance newMaintenance = new Maintenance
                {
                    ItemId = maintenanceDTO.itemId,
                    Item = item,
                    StartDate = maintenanceDTO.startDate,
                    EndDate = maintenanceDTO.endDate,
                    CreatedClerkId = clerkDto.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = maintenanceDTO.taskDescription,
                    CreatedAt = DateTime.Now,
                    TechnicianId = technician.UserId,
                    Technician = technician,
                    Status = "Scheduled",
                    IsActive = true
                };
                await _dbContext.maintenances.AddAsync(newMaintenance);
                await _dbContext.SaveChangesAsync();
                return StatusCode(201, new MaintenanceDetailedDTO
                {
                    maintenanceId = newMaintenance.MaintenanceId,
                    itemId = newMaintenance.ItemId,
                    itemName = newMaintenance.Item.Equipment.Name,
                    itemModel = newMaintenance.Item.Equipment.Model,
                    imageUrl = newMaintenance.Item.Equipment.ImageURL,
                    itemSerialNumber = newMaintenance.Item.SerialNumber,
                    labId = newMaintenance.Item.Equipment.LabId,
                    labName = newMaintenance.Item.Equipment.Lab.LabName,
                    startDate = newMaintenance.StartDate,
                    endDate = newMaintenance.EndDate,
                    createdClerkId = newMaintenance.CreatedClerkId,
                    createdClerkName = newMaintenance.CreatedClerk.FirstName + " " + newMaintenance.CreatedClerk.LastName,
                    taskDescription = newMaintenance.TaskDescription,
                    createdAt = newMaintenance.CreatedAt,
                    technicianId = newMaintenance.TechnicianId,
                    technicianName = newMaintenance.Technician.FirstName + " " + newMaintenance.Technician.LastName,
                    status = newMaintenance.Status
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPatch("maintenance/{id}")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<MaintenanceDetailedDTO>> ReviewMaintenance([FromBody] JsonElement jsonBody, [FromQuery] bool accepted, int id)
        {
            try
            {
                // Get the User from the token
                UserDTO? clerkDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (clerkDto == null) throw new Exception("Invalid Token/Authorization Header");
                User clerk = await _dbContext.users.Where(dbUser => dbUser.IsActive && dbUser.UserId == clerkDto.UserId).FirstAsync();
                // Parse the JSON
                ReviewMaintenanceDTO maintenanceDTO = new ReviewMaintenanceDTO(jsonBody, accepted);
                ValidationDTO validationDTO = maintenanceDTO.Validate();
                if (!validationDTO.success) return BadRequest(validationDTO.message);
                // Get the maintenance if Available
                Maintenance? maintenance = await _dbContext.maintenances.Where(mnt => mnt.MaintenanceId == id && mnt.IsActive && mnt.Status == "UnderReview").FirstAsync();
                if (maintenance == null) return BadRequest("Maintenance not Available for Review");
                maintenance.ReviewedClerkId = clerkDto.UserId;
                maintenance.ReviewedClerk = clerk;
                maintenance.ReviewNote = maintenanceDTO.reviewNote;
                maintenance.ReviewedAt = DateTime.Now;
                maintenance.Status = accepted ? "Completed" : "Ongoing";
                Item item = maintenance.Item;
                item.Status = accepted ? "Available" : "UnderRepair";
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
                User clerk = await _dbContext.users.Where(dbUser => dbUser.IsActive && dbUser.UserId == clerkDto.UserId).FirstAsync();
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
                    reservation.RespondedClerkId = clerkDto.UserId;
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
                reservation.RespondedClerkId = clerkDto.UserId;
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
                User clerk = await _dbContext.users.Where(dbUser => dbUser.IsActive && dbUser.UserId == clerkDto.UserId).FirstAsync();
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
                reservation.LentClerkId = clerkDto.UserId;
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
                User clerk = await _dbContext.users.Where(dbUser => dbUser.IsActive && dbUser.UserId == clerkDto.UserId).FirstAsync();
                // Get the reservation if Available
                ItemReservation? reservation = await _dbContext.itemReservations.Where(rsv => rsv.ItemReservationId == id && rsv.IsActive && rsv.Status == "Borrowed").FirstAsync();
                if (reservation == null) return BadRequest("Reservation not Available for Returning");
                // Get the token
                string? token = await _qRTokenProvider.getQRToken(reservation.ItemReservationId, clerkDto.UserId, true);
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

