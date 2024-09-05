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

		public ClerkController(DataBaseContext dbContext, ITokenParser tokenParser)
        {
            _dbContext = dbContext;
            _tokenParser = tokenParser;
        }


        [HttpPost("equipments")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<EquipmentDTO>> CreateEquipment([FromBody] JsonElement jsonBody)
        {
            try {
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
                return StatusCode(201, new EquipmentDTO
                {
                    equipmentId = newEquipment.EquipmentId,
                    name = newEquipment.Name,
                    model = newEquipment.Model,
                    labId = newEquipment.LabId,
                    imageURL = newEquipment.ImageURL,
                    specification = newEquipment.Specification,
                    maintenanceIntervalDays = newEquipment.MaintenanceIntervalDays
                });
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpPatch("equipments/{id}")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<EquipmentDTO>> UpdateEquipment([FromBody] JsonElement jsonBody, int id)
        {
            try {
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
                return Ok(new EquipmentDTO
                {
                    equipmentId = equipment.EquipmentId,
                    name = equipment.Name,
                    model = equipment.Model,
                    labId = equipment.LabId,
                    imageURL = equipment.ImageURL,
                    specification = equipment.Specification,
                    maintenanceIntervalDays = equipment.MaintenanceIntervalDays
                });
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        

        [HttpDelete("equipments/{id}")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<EquipmentDTO>> DeleteEquipment(int id)
        {
            try {
                // Delete the Equipment
                Equipment? equipment = await _dbContext.equipments.Where(e => e.EquipmentId == id && e.IsActive).FirstOrDefaultAsync();
                if (equipment == null) return BadRequest("Equipment Not Found");
                equipment.IsActive = false;
                // Delete all items of the equipment
                List<Item> items = await _dbContext.items.Where(i => i.EquipmentId == id).ToListAsync();
                foreach (Item item in items) {
                    item.IsActive = false;
                }
                // Delete all maintenance of items of the equipment
                List<Maintenance> maintenances = await _dbContext.maintenances.Where(m => m.Item.EquipmentId == id).ToListAsync();
                foreach (Maintenance maintenance in maintenances) {
                    maintenance.IsActive = false;
                }
                // Delete all reservations of items of the equipment
                List<ItemReservation> reservations = await _dbContext.itemReservations.Where(ir => ir.Item != null && ir.Item.EquipmentId == id).ToListAsync();
                foreach (ItemReservation reservation in reservations) {
                    reservation.IsActive = false;
                }
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpPost("items")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<ItemDTO>> CreateItem([FromBody] JsonElement jsonBody)
        {
            try {
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
                return StatusCode(201, new ItemDTO
                {
                    itemId = newItem.ItemId,
                    equipmentId = newItem.EquipmentId,
                    serialNumber = newItem.SerialNumber,
                    status = newItem.Status
                });
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        

        [HttpDelete("items/{id}")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<EquipmentDTO>> DeleteItem(int id)
        {
            try {
                // Delete the Item
                Item? item = await _dbContext.items.Where(i => i.ItemId == id && i.IsActive).FirstOrDefaultAsync();
                if (item == null) return BadRequest("Item Not Found");
                item.IsActive = false;
                // Delete all maintenance of the item
                List<Maintenance> maintenances = await _dbContext.maintenances.Where(m => m.ItemId == id).ToListAsync();
                foreach (Maintenance maintenance in maintenances) {
                    maintenance.IsActive = false;
                }
                // Delete all reservations of the item
                List<ItemReservation> reservations = await _dbContext.itemReservations.Where(ir => ir.ItemId == id).ToListAsync();
                foreach (ItemReservation reservation in reservations) {
                    reservation.IsActive = false;
                }
                await _dbContext.SaveChangesAsync();
                return NoContent();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("maintenance")]
		[AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<MaintenanceDTO>> CreateMaintenance([FromBody] JsonElement jsonBody)
        {
            try {
                // Get the User from the token
                UserDTO? clerkDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (clerkDto == null) throw new Exception("Invalid Token/Authorization Header");
                User clerk = await _dbContext.users.Where(dbUser => dbUser.IsActive && dbUser.UserId == clerkDto.UserId).FirstAsync();
                // Parse the JSON
                CreateMaintenanceDTO maintenanceDTO = new CreateMaintenanceDTO(jsonBody);
                ValidationDTO validationDTO = maintenanceDTO.Validate();
                if (!validationDTO.success) return BadRequest(validationDTO.message);
                // Get the item if Available
                Item? item = await _dbContext.items.Where(it => it.ItemId == maintenanceDTO.itemId && it.IsActive && it.Status == "Available").FirstAsync();
                if (item == null) return BadRequest("Item not Available for Maintenance");
                // Get the technician
                User? technician = await _dbContext.users.Where(u => u.UserId == maintenanceDTO.technicianId && u.IsActive && u.Role == "Technician").FirstAsync();
                if (technician == null) return BadRequest("Technician not Found");
                // Create the maintenance
                Maintenance newMaintenance = new Maintenance
                {
                    ItemId = maintenanceDTO.itemId,
                    Item = item,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Parse(maintenanceDTO.endDate),
                    CreatedClerkId = clerkDto.UserId,
                    CreatedClerk = clerk,
                    TaskDescription = maintenanceDTO.taskDescription,
                    CreatedAt = DateTime.Now,
                    TechnicianId = technician.UserId,
                    Technician = technician,
                    Status = "Ongoing",
                    IsActive = true
                };
                await _dbContext.maintenances.AddAsync(newMaintenance);
                await _dbContext.SaveChangesAsync();
                return StatusCode(201, new MaintenanceDTO
                {
                    maintenanceId = newMaintenance.MaintenanceId,
                    itemId = newMaintenance.ItemId,
                    startDate = newMaintenance.StartDate,
                    endDate = newMaintenance.EndDate,
                    createdClerkId = newMaintenance.CreatedClerkId,
                    taskDescription = newMaintenance.TaskDescription,
                    createdAt = newMaintenance.CreatedAt,
                    technicianId = newMaintenance.TechnicianId,
                    status = newMaintenance.Status
                });
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
		}


        [HttpPatch("maintenance/{id}")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<MaintenanceDTO>> ReviewMaintenance([FromBody] JsonElement jsonBody, [FromQuery] bool accepted, int id)
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
                await _dbContext.SaveChangesAsync();
                return Ok(new MaintenanceDTO
                {
                    maintenanceId = maintenance.MaintenanceId,
                    itemId = maintenance.ItemId,
                    startDate = maintenance.StartDate,
                    endDate = maintenance.EndDate,
                    createdClerkId = maintenance.CreatedClerkId,
                    taskDescription = maintenance.TaskDescription,
                    createdAt = maintenance.CreatedAt,
                    technicianId = maintenance.TechnicianId,
                    submitNote = maintenance.SubmitNote,
                    submittedAt = maintenance.SubmittedAt,
                    reviewedClerkId = maintenance.ReviewedClerkId,
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
                List<MaintenanceDTO> maintenanceDTOs = await _dbContext.maintenances.Where(mnt => mnt.IsActive && (completed ? mnt.Status == "Completed" : (mnt.Status == "Ongoing" || mnt.Status == "UnderReview"))).Select(mnt => new MaintenanceDTO
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


    }
}

