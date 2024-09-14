using System.Diagnostics;
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
        private readonly ReservationService _reservationService;
        private readonly UserService _userService;

        public StudentController(
            ITokenParser tokenParser,
            IQRTokenProvider qRTokenProvider,
            ILogger<StudentController> logger,
            ReservationService reservationService,
            UserService userService
        )
        {
            _tokenParser = tokenParser;
            _qRTokenProvider = qRTokenProvider;
            _logger = logger;
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
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
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
                if (!responseDTO.success)
                    return BadRequest(responseDTO.message);
                return Ok(responseDTO.result);
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
                if (!responseDTO.success)
                    return BadRequest(responseDTO.message);
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
                    studentDto.userId,
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
                if (decodedQRToken.eventId == null || decodedQRToken.isReservation != true)
                    return BadRequest("Invalid Token");
                // Get the reservationDTO if Available
                ItemReservationDetailedDTO? itemReservationDTO =
                    _reservationService.GetReservationById(id);
                if (itemReservationDTO == null || itemReservationDTO.status != "Borrowed")
                    return BadRequest("Item not Available for Returning");
                // Verify the reservation
                if (itemReservationDTO.reservedUserId != studentDto.userId)
                    return BadRequest("Only Borrowed User Can Return Item");
                UserDTO? clerk = _userService.GetUserById(decodedQRToken.userId.Value);
                if (clerk == null || clerk.role != "Clerk")
                    return BadRequest("Invalid Clerk User");
                if (decodedQRToken.eventId != itemReservationDTO.reservationId)
                    return BadRequest("Invalid Token");
                // Borrow the item
                ResponseDTO<ItemReservationDetailedDTO> responseDTO =
                    _reservationService.ReturnBorrowedItem(id, clerk.userId);
                if (!responseDTO.success)
                    return BadRequest(responseDTO.message);
                return Ok(new QRTokenValidatedDTO());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
