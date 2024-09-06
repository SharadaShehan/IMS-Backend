using Microsoft.AspNetCore.Mvc;
using IMS.Infrastructure.Services;
using System.Diagnostics;
using IMS.Presentation.Filters;
using IMS.ApplicationCore.DTO;
using IMS.Presentation.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using IMS.ApplicationCore.Model;
using System.Text.RegularExpressions;

namespace IMS.Presentation.Controllers
{
	[Route("api/admin")]
	[ApiController]
	public class AdminController: ControllerBase
    {
		private readonly DataBaseContext _dbContext;
        private readonly ITokenParser _tokenParser;

		public AdminController(DataBaseContext dbContext, ITokenParser tokenParser)
        {
            _dbContext = dbContext;
            _tokenParser = tokenParser;
        }

		[HttpGet("users")]
		[AuthorizationFilter(["SystemAdmin"])]
        public async Task<ActionResult<List<UserDTO>>> GetUsersList()
		{
            try {
                // Get the list of users
                List<UserDTO> users = await _dbContext.users.Where(dbUser => dbUser.IsActive).Select(dbUser => new UserDTO
                {
                    UserId = dbUser.UserId,
                    Email = dbUser.Email,
                    FirstName = dbUser.FirstName,
                    LastName = dbUser.LastName,
                    ContactNumber = dbUser.ContactNumber,
                    Role = dbUser.Role
                }).ToListAsync();
                return Ok(users);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
		}

        [HttpPatch("users/{id}")]
        [AuthorizationFilter(["SystemAdmin"])]
        public async Task<ActionResult<UserDTO>> UpdateUserRole([FromQuery] string role, int id)
        {
            try
            {
                string userRolePattern = @"^(Clerk|Technician|Student|AcademicStaff|SystemAdmin)$";
                // Get the user
                User? user = await _dbContext.users.Where(dbUser => dbUser.IsActive && dbUser.UserId == id).FirstAsync();
                if (user == null) { return NotFound("User not found"); }
                // check if the role is valid
                if (!Regex.IsMatch(role, userRolePattern)) throw new Exception("Invalid User Role");
                // Update the user role
                user.Role = role;
                await _dbContext.SaveChangesAsync();
                return Ok(new UserDTO
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ContactNumber = user.ContactNumber,
                    Role = user.Role
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("labs")]
        [AuthorizationFilter(["SystemAdmin"])]
        public async Task<ActionResult<UserDTO>> CreateLab([FromBody] JsonElement jsonBody)
        {
            try
            {
                // Parse the JSON
                CreateLabDTO labDTO = new CreateLabDTO(jsonBody);
                ValidationDTO validationDTO = labDTO.Validate();
                if (!validationDTO.success) return BadRequest(validationDTO.message);
                // Prevent Duplicate Lab Creation
                Lab? existingLab = await _dbContext.labs.Where(l => l.LabName == labDTO.labName && l.LabCode == labDTO.labCode && l.IsActive).FirstOrDefaultAsync();
                if (existingLab != null) return BadRequest("Lab Already Exists");
                // Create the Lab
                Lab lab = new Lab
                {
                    LabName = labDTO.labName,
                    LabCode = labDTO.labCode,
                    ImageURL = labDTO.imageURL,
                    IsActive = true
                };
                await _dbContext.labs.AddAsync(lab);
                await _dbContext.SaveChangesAsync();
                return StatusCode(201, new LabDTO
                {
                    labId = lab.LabId,
                    labName = lab.LabName,
                    labCode = lab.LabCode,
                    imageUrl = lab.ImageURL
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("labs/{id}")]
        [AuthorizationFilter(["SystemAdmin"])]
        public async Task<ActionResult<LabDTO>> UpdateLab([FromBody] JsonElement jsonBody, int id)
        {
            try
            {
                // Parse the JSON
                UpdateLabDTO labDTO = new UpdateLabDTO(jsonBody);
                ValidationDTO validationDTO = labDTO.Validate();
                if (!validationDTO.success) return BadRequest(validationDTO.message);
                // Get the Lab to be Updated
                Lab? lab = await _dbContext.labs.Where(l => l.LabId == id && l.IsActive).FirstOrDefaultAsync();
                if (lab == null) return BadRequest("Lab Not Found");
                // Update the Lab
                if (labDTO.labName != null) lab.LabName = labDTO.labName;
                if (labDTO.labCode != null) lab.LabCode = labDTO.labCode;
                if (labDTO.imageURL != null) lab.ImageURL = labDTO.imageURL;
                await _dbContext.SaveChangesAsync();
                return Ok(new LabDTO
                {
                    labId = lab.LabId,
                    labName = lab.LabName,
                    labCode = lab.LabCode,
                    imageUrl = lab.ImageURL
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("labs/{id}")]
        [AuthorizationFilter(["SystemAdmin"])]
        public async Task<ActionResult<LabDTO>> DeleteLab(int id)
        {
            try
            {
                // Delete the Lab
                Lab? lab = await _dbContext.labs.Where(l => l.LabId == id && l.IsActive).FirstOrDefaultAsync();
                if (lab == null) return BadRequest("Lab Not Found");
                lab.IsActive = false;
                // Delete all equipments of the lab
                List<Equipment> equipments = await _dbContext.equipments.Where(e => e.LabId == id).ToListAsync();
                foreach (Equipment equipment in equipments)
                {
                    equipment.IsActive = false;
                }
                // Delete all items of the lab
                List<Item> items = await _dbContext.items.Where(i => i.Equipment.LabId == id).ToListAsync();
                foreach (Item item in items)
                {
                    item.IsActive = false;
                }
                // Delete all maintenance of items of the lab
                List<Maintenance> maintenances = await _dbContext.maintenances.Where(m => m.Item.Equipment.LabId == id).ToListAsync();
                foreach (Maintenance maintenance in maintenances)
                {
                    maintenance.IsActive = false;
                }
                // Delete all reservations of items of the lab
                List<ItemReservation> reservations = await _dbContext.itemReservations.Where(ir => ir.Item != null && ir.Item.Equipment.LabId == id).ToListAsync();
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
    }
}
