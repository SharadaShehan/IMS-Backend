using FluentValidation;
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
        private readonly IValidator<CreateEquipmentDTO> _createEquipmentValidator;
        private readonly IValidator<UpdateEquipmentDTO> _updateEquipmentValidator;
        private readonly IValidator<CreateItemDTO> _createItemValidator;
        private readonly IValidator<CreateMaintenanceDTO> _createMaintenanceValidator;
        private readonly IValidator<ReviewMaintenanceDTO> _reviewMaintenanceValidator;
        private readonly IValidator<RespondReservationDTO> _respondReservationValidator;
        private readonly EquipmentService _equipmentService;
        private readonly ItemService _itemService;
        private readonly MaintenanceService _maintenanceService;
        private readonly ReservationService _reservationService;
        private readonly UserService _userService;

        public ClerkController(
            ITokenParser tokenParser,
            IQRTokenProvider qRTokenProvider,
            ILogger<ClerkController> logger,
            IValidator<CreateEquipmentDTO> createEquipmentValidator,
            IValidator<UpdateEquipmentDTO> updateEquipmentValidator,
            IValidator<CreateItemDTO> createItemValidator,
            IValidator<CreateMaintenanceDTO> createMaintenanceValidator,
            IValidator<ReviewMaintenanceDTO> reviewMaintenanceValidator,
            IValidator<RespondReservationDTO> respondReservationValidator,
            EquipmentService equipmentService,
            ItemService itemService,
            MaintenanceService maintenanceService,
            ReservationService reservationService,
            UserService userService
        )
        {
            _tokenParser = tokenParser;
            _qRTokenProvider = qRTokenProvider;
            _logger = logger;
            _createEquipmentValidator = createEquipmentValidator;
            _updateEquipmentValidator = updateEquipmentValidator;
            _createItemValidator = createItemValidator;
            _createMaintenanceValidator = createMaintenanceValidator;
            _reviewMaintenanceValidator = reviewMaintenanceValidator;
            _respondReservationValidator = respondReservationValidator;
            _equipmentService = equipmentService;
            _itemService = itemService;
            _maintenanceService = maintenanceService;
            _reservationService = reservationService;
            _userService = userService;
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
                var result = await _createEquipmentValidator.ValidateAsync(createEquipmentDTO);
                if (!result.IsValid)
                    return BadRequest(result.Errors);
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
                var result = await _updateEquipmentValidator.ValidateAsync(updateEquipmentDTO);
                if (!result.IsValid)
                    return BadRequest(result.Errors);
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
                var result = await _createItemValidator.ValidateAsync(createItemDTO);
                if (!result.IsValid)
                    return BadRequest(result.Errors);
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
                var result = await _createMaintenanceValidator.ValidateAsync(createMaintenanceDTO);
                if (!result.IsValid)
                {
                    // Create a ValidationProblemDetails object
                    ValidationProblemDetails validationProblemDetails = new ValidationProblemDetails
                    {
                        Status = 400,
                        Title = "One or more validation errors occurred.",
                        Instance = Guid.NewGuid().ToString(),
                        Type = "https://httpstatuses.com/400",
                    };
                    // Loop through FluentValidation errors and add them to problemDetails
                    foreach (var error in result.Errors)
                    {
                        if (!validationProblemDetails.Errors.ContainsKey(error.PropertyName))
                        {
                            validationProblemDetails.Errors[error.PropertyName] =
                            [
                                error.ErrorMessage,
                            ];
                        }
                        validationProblemDetails
                            .Errors[error.PropertyName]
                            .Append(error.ErrorMessage);
                    }
                    // Return the ValidationProblemDetails object
                    return BadRequest(validationProblemDetails);
                }
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
                var result = await _reviewMaintenanceValidator.ValidateAsync(reviewMaintenanceDTO);
                if (!result.IsValid)
                {
                    // Create a ValidationProblemDetails object
                    ValidationProblemDetails validationProblemDetails = new ValidationProblemDetails
                    {
                        Status = 400,
                        Title = "One or more validation errors occurred.",
                        Instance = Guid.NewGuid().ToString(),
                        Type = "https://httpstatuses.com/400",
                    };
                    // Loop through FluentValidation errors and add them to problemDetails
                    foreach (var error in result.Errors)
                    {
                        if (!validationProblemDetails.Errors.ContainsKey(error.PropertyName))
                        {
                            validationProblemDetails.Errors[error.PropertyName] =
                            [
                                error.ErrorMessage,
                            ];
                        }
                        validationProblemDetails
                            .Errors[error.PropertyName]
                            .Append(error.ErrorMessage);
                    }
                    // Return the ValidationProblemDetails object
                    return BadRequest(validationProblemDetails);
                }
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
                var result = await _respondReservationValidator.ValidateAsync(
                    respondReservationDTO
                );
                if (!result.IsValid)
                {
                    // Create a ValidationProblemDetails object
                    ValidationProblemDetails validationProblemDetails = new ValidationProblemDetails
                    {
                        Status = 400,
                        Title = "One or more validation errors occurred.",
                        Instance = Guid.NewGuid().ToString(),
                        Type = "https://httpstatuses.com/400",
                    };
                    // Loop through FluentValidation errors and add them to problemDetails
                    foreach (var error in result.Errors)
                    {
                        if (!validationProblemDetails.Errors.ContainsKey(error.PropertyName))
                        {
                            validationProblemDetails.Errors[error.PropertyName] =
                            [
                                error.ErrorMessage,
                            ];
                        }
                        validationProblemDetails
                            .Errors[error.PropertyName]
                            .Append(error.ErrorMessage);
                    }
                    // Return the ValidationProblemDetails object
                    return BadRequest(validationProblemDetails);
                }
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
                if (decodedQRToken.reservationId == null)
                    return BadRequest("Invalid Token");
                // Get the reservationDTO if Available
                ItemReservationDetailedDTO? itemReservationDTO =
                    _reservationService.GetReservationById(id);
                if (itemReservationDTO == null || itemReservationDTO.status != "Reserved")
                    return BadRequest("Reservation not Available for Borrowing");
                // Verify the reservation
                if (decodedQRToken.userId != itemReservationDTO.reservedUserId)
                    return BadRequest("Invalid Token");
                if (decodedQRToken.reservationId != itemReservationDTO.reservationId)
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
                    clerkDto.userId
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

        [HttpGet("technicians")]
        [AuthorizationFilter(["Clerk"])]
        public async Task<ActionResult<List<UserDTO>>> GetUsersList()
        {
            try
            {
                return _userService.GetAllTechnicians();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
