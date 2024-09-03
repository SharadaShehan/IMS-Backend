using Microsoft.AspNetCore.Mvc;
using IMS.Infrastructure.Services;
using System.Diagnostics;
using IMS.Presentation.Filters;
using IMS.ApplicationCore.DTO;
using IMS.Presentation.Services;
using Microsoft.EntityFrameworkCore;

namespace IMS.Presentation.Controllers
{
	[Route("api/student")]
	[ApiController]
	public class StudentController : ControllerBase
    {
		private readonly DataBaseContext _dbContext;
        private readonly ITokenParser _tokenParser;

		public StudentController(DataBaseContext dbContext, ITokenParser tokenParser)
        {
            _dbContext = dbContext;
            _tokenParser = tokenParser;
        }

		[HttpPost("reservations")]
		[AuthorizationFilter(["Student", "AcademicStaff"])]
        public async Task<ActionResult<List<UserDTO>>> RequestReservation()
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
