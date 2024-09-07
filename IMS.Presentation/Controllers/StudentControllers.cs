using Microsoft.AspNetCore.Mvc;
using IMS.Infrastructure.Services;
using System.Diagnostics;
using IMS.Presentation.Filters;
using IMS.ApplicationCore.DTO;
using IMS.Presentation.Services;
using Microsoft.EntityFrameworkCore;
using IMS.ApplicationCore.Model;
using System.Text.Json;

namespace IMS.Presentation.Controllers
{
	[Route("api/student")]
	[ApiController]
	public class StudentController : ControllerBase
    {
		private readonly DataBaseContext _dbContext;
        private readonly ITokenParser _tokenParser;
        private readonly IQRTokenProvider _qRTokenProvider;

        public StudentController(DataBaseContext dbContext, ITokenParser tokenParser, IQRTokenProvider qRTokenProvider)
        {
            _dbContext = dbContext;
            _tokenParser = tokenParser;
            _qRTokenProvider = qRTokenProvider;
        }

		[HttpPost("reservations")]
		[AuthorizationFilter(["Student", "AcademicStaff"])]
        public async Task<ActionResult<ItemReservationDTO>> RequestReservation([FromBody] JsonElement jsonBody)
		{
            try {
                // Get the User from the token
                UserDTO? studentDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (studentDto == null) throw new Exception("Invalid Token/Authorization Header");
                User student = await _dbContext.users.Where(dbUser => dbUser.IsActive && dbUser.UserId == studentDto.userId).FirstAsync();
                // Parse the JSON
                RequestEquipmentDTO reservationDTO = new RequestEquipmentDTO(jsonBody);
                ValidationDTO validationDTO = reservationDTO.Validate();
                if (!validationDTO.success) return BadRequest(validationDTO.message);
                // Get the equipment if Available
                Equipment? equipment = await _dbContext.equipments.Where(e => e.EquipmentId == reservationDTO.equipmentId && e.IsActive).FirstAsync();
                if (equipment == null) return BadRequest("Equipment not Found");
                // Create the reservation
                ItemReservation newItemReservation = new ItemReservation
                {
                    EquipmentId = reservationDTO.equipmentId,
                    Equipment = equipment,
                    StartDate = reservationDTO.startDate,
                    EndDate = reservationDTO.endDate,
                    ReservedUserId = studentDto.userId,
                    ReservedUser = student,
                    CreatedAt = DateTime.Now,
                    Status = "Pending",
                    IsActive = true
                };
                await _dbContext.itemReservations.AddAsync(newItemReservation);
                await _dbContext.SaveChangesAsync();
                return StatusCode(201, new ItemReservationDTO
                {
                    reservationId = newItemReservation.ItemReservationId,
                    equipmentId = newItemReservation.EquipmentId,
                    itemName = newItemReservation.Equipment.Name,
                    itemModel = newItemReservation.Equipment.Model,
                    imageUrl = newItemReservation.Equipment.ImageURL,
                    itemId = newItemReservation.ItemId,
                    itemSerialNumber = newItemReservation.Item != null ? newItemReservation.Item.SerialNumber : null,
                    labId = newItemReservation.Equipment.LabId,
                    labName = newItemReservation.Equipment.Lab.LabName,
                    startDate = newItemReservation.StartDate,
                    endDate = newItemReservation.EndDate,
                    reservedUserId = newItemReservation.ReservedUserId,
                    reservedUserName = newItemReservation.ReservedUser.FirstName + " " + newItemReservation.ReservedUser.LastName,
                    createdAt = newItemReservation.CreatedAt,
                    respondedAt = newItemReservation.RespondedAt,
                    borrowedAt = newItemReservation.BorrowedAt,
                    returnedAt = newItemReservation.ReturnedAt,
                    cancelledAt = newItemReservation.CancelledAt,
                    status = newItemReservation.Status
                });
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
		}

        [HttpGet("reservations")]
        [AuthorizationFilter(["Student", "AcademicStaff"])]
        public async Task<ActionResult<List<ItemReservationDTO>>> ViewReservations([FromQuery] bool borrowed)
        {
            try
            {
                // Get item reservations from DB
                List<ItemReservationDTO> reservationDTOs = await _dbContext.itemReservations.Where(rsv => rsv.IsActive && ( borrowed ? rsv.Status == "Borrowed" : ( rsv.Status == "Reserved" || rsv.Status == "Rejected" || rsv.Status == "Pending"))).Select(rsv => new ItemReservationDTO
                {
                    reservationId = rsv.ItemReservationId,
                    equipmentId = rsv.EquipmentId,
                    itemName = rsv.Equipment.Name,
                    itemModel = rsv.Equipment.Model,
                    imageUrl = rsv.Equipment.ImageURL,
                    itemId = rsv.ItemId,
                    itemSerialNumber = rsv.Item != null ? rsv.Item.SerialNumber : null,
                    labId = rsv.Equipment.LabId,
                    labName = rsv.Equipment.Lab.LabName,
                    startDate = rsv.StartDate,
                    endDate = rsv.EndDate,
                    reservedUserId = rsv.ReservedUserId,
                    reservedUserName = rsv.ReservedUser.FirstName + " " + rsv.ReservedUser.LastName,
                    createdAt = rsv.CreatedAt,
                    respondedAt = rsv.RespondedAt,
                    borrowedAt = rsv.BorrowedAt,
                    returnedAt = rsv.ReturnedAt,
                    cancelledAt = rsv.CancelledAt,
                    status = rsv.Status
                }).OrderByDescending(rsv => rsv.createdAt).ToListAsync();
                return Ok(reservationDTOs);
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
                // Get the User from the token
                UserDTO? studentDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (studentDto == null) throw new Exception("Invalid Token/Authorization Header");
                User student = await _dbContext.users.Where(dbUser => dbUser.IsActive && dbUser.UserId == studentDto.userId).FirstAsync();
                // Get the reservation
                ItemReservation? reservation = await _dbContext.itemReservations.Where(rsv => rsv.IsActive && rsv.ItemReservationId == id && rsv.ReservedUserId == studentDto.userId && rsv.Status != "Borrowed").FirstAsync();
                if (reservation == null) return BadRequest("Reservation not Available for Cancellation");
                // Cancel the reservation
                reservation.IsActive = false;
                reservation.Status = "Canceled";
                reservation.CancelledAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();
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
                // Get the User from the token
                UserDTO? studentDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (studentDto == null) throw new Exception("Invalid Token/Authorization Header");
                User student = await _dbContext.users.Where(dbUser => dbUser.IsActive && dbUser.UserId == studentDto.userId).FirstAsync();
                // Get the reservation if Available
                ItemReservation? reservation = await _dbContext.itemReservations.Where(rsv => rsv.ItemReservationId == id && rsv.IsActive && rsv.Status == "Reserved").FirstAsync();
                if (reservation == null) return BadRequest("Reservation not Available for Borrowing");
                // Get the token
                string? token = await _qRTokenProvider.getQRToken(reservation.ItemReservationId, studentDto.userId, true);
                if (token == null) return BadRequest("Token Generation Failed");
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
        public async Task<ActionResult<QRTokenValidatedDTO>> VerifyItemReturning([FromQuery] string token, int id)
        {
            try
            {
                // Get the User from the token
                UserDTO? studentDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (studentDto == null) throw new Exception("Invalid Token/Authorization Header");
                User student = await _dbContext.users.Where(dbUser => dbUser.IsActive && dbUser.UserId == studentDto.userId).FirstAsync(); 
                // Get the reservation if Available
                ItemReservation? reservation = await _dbContext.itemReservations.Where(rsv => rsv.ItemReservationId == id && rsv.IsActive && rsv.Status == "Borrowed").FirstAsync();
                if (reservation == null) return BadRequest("Reservation not Available for Returning");
                // Verify the token
                DecodedQRToken decodedQRToken = await _qRTokenProvider.validateQRToken(token);
                if (!decodedQRToken.success) return BadRequest(decodedQRToken.message);
                if (decodedQRToken.eventId == null) return BadRequest("Invalid Token");
                if (decodedQRToken.isReservation != true) return BadRequest("Invalid Token");
                // Verify the reservation
                User? clerk = await _dbContext.users.Where(u => u.IsActive && u.UserId == decodedQRToken.userId && u.Role == "Clerk").FirstAsync();
                if (clerk == null) return BadRequest("Invalid Clerk User");
                if (decodedQRToken.eventId != reservation.ItemReservationId) return BadRequest("Invalid Token");
                // Borrow the item
                reservation.ReturnAcceptedClerk = clerk;
                reservation.ReturnAcceptedClerkId = clerk.UserId;
                reservation.ReturnedAt = DateTime.Now;
                reservation.Status = "Returned";
                Item borrowedItem = reservation.Item;
                borrowedItem.Status = "Available";
                await _dbContext.SaveChangesAsync();
                return Ok(new QRTokenValidatedDTO());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        

    }
}
