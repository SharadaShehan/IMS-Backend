using System.Diagnostics;
using IMS.Application.DTO;
using IMS.Application.Services;
using IMS.Presentation.Filters;
using IMS.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Presentation.Controllers
{
    [Route("api/clerk")]
    [ApiController]
    public class ClerkController : ControllerBase
    {
        private readonly ITokenParser _tokenParser;
        private readonly IQRTokenProvider _qRTokenProvider;
        private readonly ILogger<ClerkController> _logger;
        private readonly EquipmentService _equipmentService;
        private readonly ItemService _itemService;
        private readonly MaintenanceService _maintenanceService;
        private readonly ReservationService _reservationService;

        public ClerkController(
            ITokenParser tokenParser,
            IQRTokenProvider qRTokenProvider,
            ILogger<ClerkController> logger,
            EquipmentService equipmentService,
            ItemService itemService,
            MaintenanceService maintenanceService,
            ReservationService reservationService
        )
        {
            _tokenParser = tokenParser;
            _qRTokenProvider = qRTokenProvider;
            _logger = logger;
            _equipmentService = equipmentService;
            _itemService = itemService;
            _maintenanceService = maintenanceService;
            _reservationService = reservationService;
        }

        [HttpPost("equipments")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<EquipmentDetailedDTO>> CreateEquipment(
            CreateEquipmentDTO createEquipmentDTO
        )
        {
            try
            {
                // Validate the DTO
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                // Create the Equipment
                ResponseDTO<EquipmentDetailedDTO> responseDTO =
                    _equipmentService.CreateNewEquipment(createEquipmentDTO);
                if (!responseDTO.success)
                    return BadRequest(responseDTO.message);
                return StatusCode(201, responseDTO.result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("equipments/{id}")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<EquipmentDetailedDTO>> UpdateEquipment(
            int id,
            UpdateEquipmentDTO updateEquipmentDTO
        )
        {
            try
            {
                // Validate the DTO
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                // Update the Equipment
                ResponseDTO<EquipmentDetailedDTO> responseDTO = _equipmentService.UpdateEquipment(
                    id,
                    updateEquipmentDTO
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

        [HttpDelete("equipments/{id}")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<EquipmentDTO>> DeleteEquipment(int id)
        {
            try
            {
                // Delete the Equipment
                ResponseDTO<EquipmentDetailedDTO> responseDTO = _equipmentService.DeleteEquipment(
                    id
                );
                if (!responseDTO.success)
                    return BadRequest(responseDTO.message);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("items")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<ItemDetailedDTO>> CreateItem(CreateItemDTO createItemDTO)
        {
            try
            {
                // Validate the DTO
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                // Create the Item
                ResponseDTO<ItemDetailedDTO> responseDTO = _itemService.CreateNewItem(
                    createItemDTO
                );
                if (!responseDTO.success)
                    return BadRequest(responseDTO.message);
                return StatusCode(201, responseDTO.result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("items/{id}")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<ItemDTO>> DeleteItem(int id)
        {
            try
            {
                // Delete the Item
                ResponseDTO<ItemDetailedDTO> responseDTO = _itemService.DeleteItem(id);
                if (!responseDTO.success)
                    return BadRequest(responseDTO.message);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("maintenance")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<MaintenanceDetailedDTO>> CreateMaintenance(
            CreateMaintenanceDTO createMaintenanceDTO
        )
        {
            try
            {
                // Validate the DTO
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                // Get the User from the token
                UserDTO? clerkDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (clerkDto == null)
                    throw new Exception("Invalid Token/Authorization Header");
                // Create the maintenance
                ResponseDTO<MaintenanceDetailedDTO> responseDTO =
                    _maintenanceService.CreateNewMaintenance(clerkDto.userId, createMaintenanceDTO);
                if (!responseDTO.success)
                    return BadRequest(responseDTO.message);
                return StatusCode(201, responseDTO.result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("maintenance/{id}")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<MaintenanceDetailedDTO>> ReviewMaintenance(
            int id,
            ReviewMaintenanceDTO reviewMaintenanceDTO
        )
        {
            try
            {
                // Validate the DTO
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                // Get the User from the token
                UserDTO? clerkDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (clerkDto == null)
                    throw new Exception("Invalid Token/Authorization Header");
                // Review the maintenance
                ResponseDTO<MaintenanceDetailedDTO> responseDTO =
                    _maintenanceService.ReviewMaintenance(
                        id,
                        clerkDto.userId,
                        reviewMaintenanceDTO
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

        [HttpGet("maintenance")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<List<MaintenanceDTO>>> ViewMaintenances(
            [FromQuery] bool completed
        )
        {
            try
            {
                return _maintenanceService.GetAllMaintenances(completed);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("maintenance/pending")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<List<PendingMaintenanceDTO>>> ViewPendingMaintenances()
        {
            try
            {
                return _maintenanceService.GetAllPendingMaintenances();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("reservations")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<List<ItemReservationDTO>>> ViewReservations(
            [FromQuery] bool requested,
            [FromQuery] bool reserved,
            [FromQuery] bool borrowed
        )
        {
            try
            {
                return _reservationService.GetAllReservations(requested, reserved, borrowed);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("reservations/{id}")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<ItemReservationDetailedDTO>> RespondReservation(
            int id,
            RespondReservationDTO respondReservationDTO
        )
        {
            try
            {
                // Validate the DTO
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                // Get the User from the token
                UserDTO? clerkDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (clerkDto == null)
                    throw new Exception("Invalid Token/Authorization Header");
                // Respond to the reservation
                ResponseDTO<ItemReservationDetailedDTO> responseDTO =
                    _reservationService.RespondToReservationRequest(
                        id,
                        clerkDto.userId,
                        respondReservationDTO
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

        [HttpPatch("reservations/{id}/verify")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<QRTokenValidatedDTO>> VerifyItemBorrowing(
            int id,
            [FromQuery] string token
        )
        {
            try
            {
                // Get the User from the token
                UserDTO? clerkDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (clerkDto == null)
                    throw new Exception("Invalid Token/Authorization Header");
                // Verify the token
                DecodedQRToken decodedQRToken = await _qRTokenProvider.validateQRToken(token);
                if (!decodedQRToken.success)
                    return BadRequest(decodedQRToken.message);
                if (decodedQRToken.eventId == null)
                    return BadRequest("Invalid Token");
                if (decodedQRToken.isReservation != true)
                    return BadRequest("Invalid Token");
                // Get the reservationDTO if Available
                ItemReservationDetailedDTO? itemReservationDTO =
                    _reservationService.GetReservationById(id);
                if (itemReservationDTO == null)
                    return BadRequest("Item not Available for Borrowing");
                if (itemReservationDTO.status != "Reserved")
                    return BadRequest("Item not Available for Borrowing");
                // Verify the reservation
                if (decodedQRToken.userId != itemReservationDTO.reservedUserId)
                    return BadRequest("Invalid Token");
                if (decodedQRToken.eventId != itemReservationDTO.reservationId)
                    return BadRequest("Invalid Token");
                // Borrow the item
                ResponseDTO<ItemReservationDetailedDTO> responseDTO =
                    _reservationService.BorrowReservedItem(id, clerkDto.userId);
                if (!responseDTO.success)
                    return BadRequest(responseDTO.message);
                return Ok(new QRTokenValidatedDTO());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("reservations/{id}/token")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<QRTokenGeneratedDTO>> GetTokenForReturningItem(int id)
        {
            try
            {
                // Get the User from the token
                UserDTO? clerkDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (clerkDto == null)
                    throw new Exception("Invalid Token/Authorization Header");
                // Get the reservation if Available
                ItemReservationDetailedDTO? itemReservationDTO =
                    _reservationService.GetReservationById(id);
                if (itemReservationDTO == null)
                    return BadRequest("Item not Available for Returning");
                if (itemReservationDTO.status != "Borrowed")
                    return BadRequest("Item not Available for Returning");
                // Get the token
                string? token = await _qRTokenProvider.getQRToken(
                    itemReservationDTO.reservationId,
                    clerkDto.userId,
                    true
                );
                if (token == null)
                    return BadRequest("Token Generation Failed");
                // Return the token
                return Ok(new QRTokenGeneratedDTO(token));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
