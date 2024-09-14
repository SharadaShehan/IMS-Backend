using IMS.Application.DTO;
using IMS.Application.Services;
using IMS.Presentation.Filters;
using IMS.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Presentation.Controllers
{
    [Route("api/technician")]
    [ApiController]
    public class TechnicianController : ControllerBase
    {
        private readonly ITokenParser _tokenParser;
        private readonly ILogger<TechnicianController> _logger;
        private readonly MaintenanceService _maintenanceService;

        public TechnicianController(
            ITokenParser tokenParser,
            ILogger<TechnicianController> logger,
            MaintenanceService maintenanceService
        )
        {
            _tokenParser = tokenParser;
            _logger = logger;
            _maintenanceService = maintenanceService;
        }

        [HttpGet("maintenance")]
        [AuthorizationFilter(["Technician"])]
        public async Task<ActionResult<List<MaintenanceDTO>>> ViewMaintenances(
            [FromQuery] bool completed
        )
        {
            try
            {
                // Get the User from auth token
                UserDTO? technicianDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (technicianDto == null)
                    throw new Exception("Invalid Token/Authorization Header");
                // Get the Maintenances
                return _maintenanceService.GetAllMaintenancesByTechnicianId(
                    technicianDto.userId,
                    completed
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("maintenance/{id}/borrow")]
        [AuthorizationFilter(["Technician"])]
        public async Task<ActionResult<MaintenanceDTO>> BorrowItemForMaintenance(int id)
        {
            try
            {
                // Get the User from auth token
                UserDTO? technicianDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (technicianDto == null)
                    throw new Exception("Invalid Token/Authorization Header");
                // Borrow the Item
                ResponseDTO<MaintenanceDetailedDTO> responseDTO =
                    _maintenanceService.BorrowItemForMaintenance(id, technicianDto.userId);
                if (!responseDTO.success)
                    return BadRequest(responseDTO.message);
                return Ok(responseDTO.result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("maintenance/{id}")]
        [AuthorizationFilter(["Technician"])]
        public async Task<ActionResult<MaintenanceDetailedDTO>> SubmitMaintenanceUpdate(
            int id,
            SubmitMaintenanceDTO submitMaintenanceDTO
        )
        {
            try
            {
                // Validate the DTO
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                // Get the User from auth token
                UserDTO? technicianDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (technicianDto == null)
                    throw new Exception("Invalid Token/Authorization Header");
                // Submit the Maintenance Update
                ResponseDTO<MaintenanceDetailedDTO> responseDTO =
                    _maintenanceService.SubmitMaintenanceUpdate(
                        id,
                        technicianDto.userId,
                        submitMaintenanceDTO
                    );
                if (!responseDTO.success)
                    return BadRequest(responseDTO.message);
                return Ok(responseDTO.result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
