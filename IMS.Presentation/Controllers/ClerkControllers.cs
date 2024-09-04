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
        public async Task<ActionResult<Maintenance>> CreateMaintenance([FromBody] JsonElement jsonBody)
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
                    StartDate = DateTime.Parse(maintenanceDTO.startDate),
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
                return StatusCode(201, newMaintenance);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
		}
	}
}
