using Microsoft.AspNetCore.Mvc;
using IMS.Infrastructure.Services;
using System.Diagnostics;
using IMS.Presentation.Filters;
using IMS.ApplicationCore.DTO;
using IMS.Presentation.Services;
using Microsoft.EntityFrameworkCore;

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

		[HttpPatch("maintenance")]
		[AuthorizationFilter(["Technician"])]
        public async Task<ActionResult<List<UserDTO>>> UpdateMaintenance()
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
	}
}
