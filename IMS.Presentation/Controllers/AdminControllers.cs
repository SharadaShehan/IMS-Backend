using Microsoft.AspNetCore.Mvc;
using IMS.Infrastructure.Services;
using System.Diagnostics;
using IMS.Presentation.Filters;
using IMS.ApplicationCore.DTO;
using IMS.Presentation.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
		[HttpPost("Users")]
		public async Task<IActionResult> AddUsers([FromBody] JsonElement jasonElemet)
        {

        }
	}
}
