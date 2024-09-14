using IMS.Application.DTO;
using IMS.Application.Services;
using IMS.Presentation.Filters;
using IMS.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Presentation.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ITokenParser _tokenParser;
        private readonly ILogger<UserController> _logger;
        private readonly UserService _userService;
        private readonly LabService _labService;
        private readonly EquipmentService _equipmentService;
        private readonly ItemService _itemService;
        private readonly MaintenanceService _maintenanceService;
        private readonly ReservationService _reservationService;

        public UserController(
            ITokenParser tokenParser,
            ILogger<UserController> logger,
            UserService userService,
            LabService labService,
            EquipmentService equipmentService,
            ItemService itemService,
            MaintenanceService maintenanceService,
            ReservationService reservationService
        )
        {
            _tokenParser = tokenParser;
            _logger = logger;
            _userService = userService;
            _labService = labService;
            _equipmentService = equipmentService;
            _itemService = itemService;
            _maintenanceService = maintenanceService;
            _reservationService = reservationService;
        }

        [HttpGet("labs")]
        [AuthorizationFilter(["Clerk", "Technician", "Student", "AcademicStaff", "SystemAdmin"])]
        public async Task<ActionResult<List<LabDTO>>> GetLabsList()
        {
            try
            {
                return _labService.GetAllLabs();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("equipments")]
        [AuthorizationFilter(["Clerk", "Technician", "Student", "AcademicStaff", "SystemAdmin"])]
        public async Task<ActionResult<List<EquipmentDTO>>> GetEquipmentsList([FromQuery] int labId)
        {
            try
            {
                return _equipmentService.GetAllEquipments(labId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("equipments/{id}")]
        [AuthorizationFilter(["Clerk", "Technician", "Student", "AcademicStaff", "SystemAdmin"])]
        public async Task<ActionResult<EquipmentDetailedDTO>> GetDetailedEquipment(int id)
        {
            try
            {
                EquipmentDetailedDTO? equipment = _equipmentService.GetEquipmentById(id);
                if (equipment == null)
                {
                    return NotFound("Equipment Not Found");
                }
                return Ok(equipment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("items")]
        [AuthorizationFilter(["Clerk", "Technician", "SystemAdmin"])]
        public async Task<ActionResult<List<ItemDTO>>> GetItemsList([FromQuery] int equipmentId)
        {
            try
            {
                return _itemService.GetAllItems(equipmentId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("items/{id}")]
        [AuthorizationFilter(["Clerk", "Technician", "SystemAdmin"])]
        public async Task<ActionResult<ItemDetailedDTO>> GetDetailedItem(int id)
        {
            try
            {
                ItemDetailedDTO? item = _itemService.GetItemById(id);
                if (item == null)
                {
                    return NotFound("Item Not Found");
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("maintenances")]
        [AuthorizationFilter(["Clerk", "Technician", "SystemAdmin"])]
        public async Task<ActionResult<List<MaintenanceDTO>>> GetMaintenancesList(
            [FromQuery] int itemId
        )
        {
            try
            {
                return _maintenanceService.GetAllMaintenances(itemId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("maintenance/{id}")]
        [AuthorizationFilter(["Clerk", "Technician", "SystemAdmin"])]
        public async Task<ActionResult<MaintenanceDetailedDTO>> ViewDetailedMaintenance(int id)
        {
            try
            {
                MaintenanceDetailedDTO? maintenance = _maintenanceService.GetMaintenanceById(id);
                if (maintenance == null)
                {
                    return NotFound("Maintenance Not Found");
                }
                return Ok(maintenance);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("reservations")]
        [AuthorizationFilter(["Clerk", "Technician", "SystemAdmin"])]
        public async Task<ActionResult<List<ItemReservationDTO>>> GetReservationsList(
            [FromQuery] int itemId
        )
        {
            try
            {
                return _reservationService.GetAllReservations(itemId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("reservations/{id}")]
        [AuthorizationFilter(["Clerk", "Technician", "SystemAdmin"])]
        public async Task<ActionResult<ItemReservationDetailedDTO>> ViewDetailedReservation(int id)
        {
            try
            {
                ItemReservationDetailedDTO? reservation = _reservationService.GetReservationById(
                    id
                );
                if (reservation == null)
                {
                    return NotFound("Reservation Not Found");
                }
                return Ok(reservation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("role")]
        [AuthorizationFilter(["Clerk", "Technician", "Student", "AcademicStaff", "SystemAdmin"])]
        public async Task<ActionResult<UserRoleDTO>> GetUserRole()
        {
            try
            {
                // Get the User from the auth token
                UserDTO? user = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (user == null)
                    throw new Exception("Invalid Token/Authorization Header");
                return Ok(new UserRoleDTO(user.role));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
