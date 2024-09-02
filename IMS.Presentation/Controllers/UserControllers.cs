using Microsoft.AspNetCore.Mvc;
using IMS.ApplicationCore.Model;
using IMS.ApplicationCore.DTO;
using IMS.Infrastructure.Services;
using System.Diagnostics;
using IMS.Presentation.Filters;
using IMS.Presentation.Services;

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
