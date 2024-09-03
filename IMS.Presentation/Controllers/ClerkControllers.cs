using Microsoft.AspNetCore.Mvc;
using IMS.Infrastructure.Services;
using System.Diagnostics;
using IMS.Presentation.Filters;
using IMS.ApplicationCore.DTO;
using IMS.Presentation.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using IMS.ApplicationCore.Model;
using System.Collections.Generic;

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

		[HttpPost("maintenance")]
		[AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<List<UserDTO>>> CreateMaintenance([FromBody] JsonElement jsonBody)
        {
            try {
                CreateMaintenanceDTO maintenanceDTO = new CreateMaintenanceDTO(jsonBody);
                ValidationDTO validationDTO = maintenanceDTO.Validate();
                if (!validationDTO.success) return BadRequest(validationDTO.message);
                // Get the item
                Item? item = await _dbContext.items.Where(it => it.ItemId == maintenanceDTO.itemId && it.IsActive).FirstOrDefaultAsync();
                if (item == null) return BadRequest("Item not Found");
                // Get the technician
                //User? technician = await _dbContext.users.Where(u => u.UserId == maintenanceDTO.assignedTechnicianId && u.IsActive).FirstOrDefaultAsync();
                //if (technician == null) return BadRequest("Technician not Found");
                //// Create the maintenance
                //Maintenance maintenance = await _dbContext.Maintenances.AddAsync(new Maintenance
                //{

                //}

                List < UserDTO > users = await _dbContext.users.Where(dbUser => dbUser.IsActive).Select(dbUser => new UserDTO
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
	}
}
