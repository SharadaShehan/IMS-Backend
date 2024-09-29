using System.Text.RegularExpressions;
using FluentValidation;
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
        private readonly IValidator<UpdateLabDTO> _updateLabDTOValidator;
        private readonly IValidator<CreateLabDTO> _createLabDTOValidator;
        private readonly UserService _userService;
        private readonly LabService _labService;

        public AdminController(
            DataBaseContext dbContext,
            ILogger<AdminController> logger,
            IValidator<UpdateLabDTO> updateLabDTOValidator,
            IValidator<CreateLabDTO> createLabDTOValidator,
            ITokenParser tokenParser,
            UserService userService,
            LabService labService
        )
        {
            _dbContext = dbContext;
            _logger = logger;
            _updateLabDTOValidator = updateLabDTOValidator;
            _createLabDTOValidator = createLabDTOValidator;
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
                // Validate New Role
                string userRolePattern = @"^(Clerk|Technician|Student|AcademicStaff|SystemAdmin)$";
                if (!Regex.IsMatch(role, userRolePattern))
                    return BadRequest("Invalid Role");
                // Get the User from auth token
                UserDTO? adminDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (adminDto == null)
                    throw new Exception("Invalid Token/Authorization Header");
                ResponseDTO<UserDTO> responseDTO = _userService.UpdateUserRole(id, role);
                if (!responseDTO.success)
                {
                    _logger.LogInformation(
                        "{UserRole} (Id:{UserId}) {Action} {ObjectType} (Id:{ObjectId}) | {Status}",
                        "ADMIN",
                        adminDto.userId,
                        "UPDATE",
                        "USER",
                        id,
                        "FAILED"
                    );
                    return BadRequest(responseDTO.message);
                }
                _logger.LogInformation(
                    "{UserRole} (Id:{UserId}) {Action} {ObjectType} (Id:{ObjectId}) | {Status}",
                    "ADMIN",
                    adminDto.userId,
                    "UPDATE",
                    "USER",
                    id,
                    "SUCCESS"
                );
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
                var result = await _createLabDTOValidator.ValidateAsync(createLabDTO);
                if (!result.IsValid)
                    return BadRequest(result.Errors);
                // Get the User from auth token
                UserDTO? adminDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (adminDto == null)
                    throw new Exception("Invalid Token/Authorization Header");
                // Create the Lab
                ResponseDTO<LabDTO> responseDTO = _labService.CreateNewLab(createLabDTO);
                if (!responseDTO.success)
                {
                    _logger.LogInformation(
                        "{UserRole} (Id:{UserId}) {Action} {ObjectType} | {Status}",
                        "ADMIN",
                        adminDto.userId,
                        "CREATE",
                        "LAB",
                        "FAILED"
                    );
                    return BadRequest(responseDTO.message);
                }
                _logger.LogInformation(
                    "{UserRole} (Id:{UserId}) {Action} {ObjectType} (Id:{ObjectId}) | {Status}",
                    "ADMIN",
                    adminDto.userId,
                    "CREATE",
                    "LAB",
                    responseDTO.result?.labId,
                    "SUCCESS"
                );
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
                var result = await _updateLabDTOValidator.ValidateAsync(updateLabDTO);
                if (!result.IsValid)
                    return BadRequest(result.Errors);
                // Get the User from auth token
                UserDTO? adminDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (adminDto == null)
                    throw new Exception("Invalid Token/Authorization Header");
                // Update the Lab
                ResponseDTO<LabDTO> responseDTO = _labService.UpdateLab(id, updateLabDTO);
                if (!responseDTO.success)
                {
                    _logger.LogInformation(
                        "{UserRole} (Id:{UserId}) {Action} {ObjectType} (Id:{ObjectId}) | {Status}",
                        "ADMIN",
                        adminDto.userId,
                        "UPDATE",
                        "LAB",
                        responseDTO.result?.labId,
                        "FAILED"
                    );
                    return BadRequest(responseDTO.message);
                }
                _logger.LogInformation(
                    "{UserRole} (Id:{UserId}) {Action} {ObjectType} (Id:{ObjectId}) | {Status}",
                    "ADMIN",
                    adminDto.userId,
                    "UPDATE",
                    "LAB",
                    responseDTO.result?.labId,
                    "SUCCESS"
                );
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
                // Get the User from auth token
                UserDTO? adminDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (adminDto == null)
                    throw new Exception("Invalid Token/Authorization Header");
                // Delete the Lab
                ResponseDTO<LabDTO> responseDTO = _labService.DeleteLab(id);
                if (!responseDTO.success)
                {
                    _logger.LogInformation(
                        "{UserRole} (Id:{UserId}) {Action} {ObjectType} (Id:{ObjectId}) | {Status}",
                        "ADMIN",
                        adminDto.userId,
                        "DELETE",
                        "LAB",
                        responseDTO.result?.labId,
                        "FAILED"
                    );
                    return BadRequest(responseDTO.message);
                }
                _logger.LogInformation(
                    "{UserRole} (Id:{UserId}) {Action} {ObjectType} (Id:{ObjectId}) | {Status}",
                    "ADMIN",
                    adminDto.userId,
                    "DELETE",
                    "LAB",
                    responseDTO.result?.labId,
                    "SUCCESS"
                );
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
