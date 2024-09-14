using System.Text.RegularExpressions;
using IMS.Application.DTO;
using IMS.Application.Services;
using IMS.Infrastructure.Services;
using IMS.Presentation.Filters;
using IMS.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Presentation.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DataBaseContext _dbContext;
        private readonly ITokenParser _tokenParser;
        private readonly ILogger<AdminController> _logger;
        private readonly UserService _userService;
        private readonly LabService _labService;

        public AdminController(
            DataBaseContext dbContext,
            ILogger<AdminController> logger,
            ITokenParser tokenParser,
            UserService userService,
            LabService labService
        )
        {
            _dbContext = dbContext;
            _logger = logger;
            _tokenParser = tokenParser;
            _userService = userService;
            _labService = labService;
        }

        [HttpGet("users")]
        [AuthorizationFilter(["SystemAdmin"])]
        public async Task<ActionResult<List<UserDTO>>> GetUsersList()
        {
            try
            {
                return _userService.GetAllUsers();
            }
            catch (Exception ex)
            {
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
                if (!Regex.IsMatch(role, userRolePattern))
                    return BadRequest("Invalid Role");
                ResponseDTO<UserDTO> responseDTO = _userService.UpdateUserRole(id, role);
                if (!responseDTO.success)
                    return BadRequest(responseDTO.message);
                return Ok(responseDTO.result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("labs")]
        [AuthorizationFilter(["SystemAdmin"])]
        public async Task<ActionResult<LabDTO>> CreateLab(CreateLabDTO createLabDTO)
        {
            try
            {
                // Validate the DTO
                if (!ModelState.IsValid)
                    BadRequest(ModelState);
                // Create the Lab
                ResponseDTO<LabDTO> responseDTO = _labService.CreateNewLab(createLabDTO);
                if (!responseDTO.success)
                    return BadRequest(responseDTO.message);
                return StatusCode(201, responseDTO.result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("labs/{id}")]
        [AuthorizationFilter(["SystemAdmin"])]
        public async Task<ActionResult<LabDTO>> UpdateLab(int id, UpdateLabDTO updateLabDTO)
        {
            try
            {
                // Validate the DTO
                if (!ModelState.IsValid)
                    BadRequest(ModelState);
                // Update the Lab
                ResponseDTO<LabDTO> responseDTO = _labService.UpdateLab(id, updateLabDTO);
                if (!responseDTO.success)
                    return BadRequest(responseDTO.message);
                return Ok(responseDTO.result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("labs/{id}")]
        [AuthorizationFilter(["SystemAdmin"])]
        public async Task<ActionResult> DeleteLab(int id)
        {
            try
            {
                // Delete the Lab
                ResponseDTO<LabDTO> responseDTO = _labService.DeleteLab(id);
                if (!responseDTO.success)
                    return BadRequest(responseDTO.message);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
