using FluentValidation;
using IMS.Application.DTO;
using IMS.Application.Services;
using IMS.Presentation.Filters;
using IMS.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Presentation.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ITokenParser _tokenParser;
        private readonly IQRTokenProvider _qRTokenProvider;
        private readonly ILogger<StudentController> _logger;
        private readonly IValidator<RequestEquipmentDTO> _requestEquipmentValidator;
        private readonly ReservationService _reservationService;
        private readonly UserService _userService;

        public StudentController(
            ITokenParser tokenParser,
            IQRTokenProvider qRTokenProvider,
            ILogger<StudentController> logger,
            IValidator<RequestEquipmentDTO> requestEquipmentValidator,
            ReservationService reservationService,
            UserService userService
        )
        {
            _tokenParser = tokenParser;
            _qRTokenProvider = qRTokenProvider;
            _logger = logger;
            _requestEquipmentValidator = requestEquipmentValidator;
            _reservationService = reservationService;
            _userService = userService;
        }

        [HttpPost("reservations")]
        [AuthorizationFilter(["Student", "AcademicStaff"])]
        public async Task<ActionResult<ItemReservationDTO>> RequestReservation(
            RequestEquipmentDTO requestEquipmentDTO
        )
        {
            try
            {
                // Validate the DTO
                var result = await _requestEquipmentValidator.ValidateAsync(requestEquipmentDTO);
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
                // Get the User from auth token
                UserDTO? studentDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (studentDto == null)
                    throw new Exception("Invalid Token/Authorization Header");
                // Create the reservation
                ResponseDTO<ItemReservationDetailedDTO> responseDTO =
                    _reservationService.CreateNewReservation(
                        studentDto.userId,
                        requestEquipmentDTO
                    );
                string role = (studentDto.role == "Student") ? "STUDENT" : "ACADEMIC_STAFF";
                if (!responseDTO.success)
                {
                    _logger.LogInformation(
                        "{UserRole} (Id:{UserId}) {Action} {ObjectType} | {Status}",
                        role,
                        studentDto.userId,
                        "CREATE",
                        "RESERVATION",
                        "FAILED"
                    );
                    return BadRequest(responseDTO.message);
                }
                _logger.LogInformation(
                    "{UserRole} (Id:{UserId}) {Action} {ObjectType} (Id:{ObjectId}) | {Status}",
                    role,
                    studentDto.userId,
                    "CREATE",
                    "RESERVATION",
                    responseDTO.result?.reservationId,
                    "SUCCESS"
                );
                return StatusCode(201, responseDTO.result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("reservations")]
        [AuthorizationFilter(["Student", "AcademicStaff"])]
        public async Task<ActionResult<List<ItemReservationDTO>>> ViewReservations(
            [FromQuery] bool borrowed
        )
        {
            try
            {
                // Get the User from auth token
                UserDTO? studentDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (studentDto == null)
                    throw new Exception("Invalid Token/Authorization Header");
                // Get the reservations
                return _reservationService.GetAllReservationsByStudent(studentDto.userId, borrowed);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("reservations/{id}")]
        [AuthorizationFilter(["Student", "AcademicStaff"])]
        public async Task<ActionResult> CancelReservation(int id)
        {
            try
            {
                // Get the User from auth token
                UserDTO? studentDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (studentDto == null)
                    throw new Exception("Invalid Token/Authorization Header");
                // Cancel the reservation
                ResponseDTO<ItemReservationDetailedDTO> responseDTO =
                    _reservationService.CancelReservation(id, studentDto.userId);
                string role = (studentDto.role == "Student") ? "STUDENT" : "ACADEMIC_STAFF";
                if (!responseDTO.success)
                {
                    _logger.LogInformation(
                        "{UserRole} (Id:{UserId}) {Action} {ObjectType} (Id:{ObjectId}) | {Status}",
                        role,
                        studentDto.userId,
                        "DELETE",
                        "RESERVATION",
                        responseDTO.result?.reservationId,
                        "FAILED"
                    );
                    return BadRequest(responseDTO.message);
                }
                _logger.LogInformation(
                    "{UserRole} (Id:{UserId}) {Action} {ObjectType} (Id:{ObjectId}) | {Status}",
                    role,
                    studentDto.userId,
                    "DELETE",
                    "RESERVATION",
                    responseDTO.result?.reservationId,
                    "SUCCESS"
                );
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("reservations/{id}/token")]
        [AuthorizationFilter(["Student", "AcademicStaff"])]
        public async Task<ActionResult<QRTokenGeneratedDTO>> GetTokenForBorrowingItem(int id)
        {
            try
            {
                // Get the User from auth token
                UserDTO? studentDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (studentDto == null)
                    throw new Exception("Invalid Token/Authorization Header");
                // Get the reservation if Available
                ItemReservationDetailedDTO? itemReservationDTO =
                    _reservationService.GetReservationById(id);
                if (itemReservationDTO == null || itemReservationDTO.status != "Reserved")
                    return BadRequest("Item not Available for Borrowing");
                // Get the QR token
                string? token = await _qRTokenProvider.getQRToken(
                    itemReservationDTO.reservationId,
                    studentDto.userId
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

        [HttpPatch("reservations/{id}/verify")]
        [AuthorizationFilter(["Student", "AcademicStaff"])]
        public async Task<ActionResult<QRTokenValidatedDTO>> VerifyItemReturning(
            int id,
            [FromQuery] string token
        )
        {
            try
            {
                // Get the User from the token
                UserDTO? studentDto = await _tokenParser.getUser(
                    HttpContext.Request.Headers["Authorization"].FirstOrDefault()
                );
                if (studentDto == null)
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
                if (itemReservationDTO == null || itemReservationDTO.status != "Borrowed")
                    return BadRequest("Reservation not Available for Borrowing");
                // Verify the reservation
                if (itemReservationDTO.reservedUserId != studentDto.userId)
                    return BadRequest("Only Borrowed User Can Return Item");
                UserDTO? clerk = _userService.GetUserById(decodedQRToken.userId.Value);
                if (clerk == null || clerk.role != "Clerk")
                    return BadRequest("Invalid Clerk User");
                if (decodedQRToken.reservationId != itemReservationDTO.reservationId)
                    return BadRequest("Invalid Token");
                // Borrow the item
                ResponseDTO<ItemReservationDetailedDTO> responseDTO =
                    _reservationService.ReturnBorrowedItem(id, clerk.userId);
                string role = (studentDto.role == "Student") ? "STUDENT" : "ACADEMIC_STAFF";
                if (!responseDTO.success)
                {
                    _logger.LogInformation(
                        "{UserRole} (Id:{UserId}) {Action} {ObjectType} (Id:{ObjectId}) | [RETURN_ITEM] | {Status}",
                        role,
                        studentDto.userId,
                        "UPDATE",
                        "RESERVATION",
                        responseDTO.result?.reservationId,
                        "FAILED"
                    );
                    return BadRequest(responseDTO.message);
                }
                _logger.LogInformation(
                    "{UserRole} (Id:{UserId}) {Action} {ObjectType} (Id:{ObjectId}) | [RETURN_ITEM] | {Status}",
                    role,
                    studentDto.userId,
                    "UPDATE",
                    "RESERVATION",
                    responseDTO.result?.reservationId,
                    "SUCCESS"
                );
                return Ok(new QRTokenValidatedDTO());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
