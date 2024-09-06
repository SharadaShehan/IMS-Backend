using Microsoft.AspNetCore.Mvc;
using IMS.Infrastructure.Services;
using System.Diagnostics;
using IMS.Presentation.Filters;
using IMS.ApplicationCore.DTO;
using IMS.Presentation.Services;
using Microsoft.EntityFrameworkCore;
using IMS.ApplicationCore.Model;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Collections.ObjectModel;

namespace IMS.Presentation.Controllers
{
	[Route("api/student")]
	[ApiController]
	public class StudentController : ControllerBase
    {
		private readonly DataBaseContext _dbContext;
        private readonly ITokenParser _tokenParser;

		public StudentController(DataBaseContext dbContext, ITokenParser tokenParser)
        {
            _dbContext = dbContext;
            _tokenParser = tokenParser;
        }

		[HttpPost("reservations")]
		[AuthorizationFilter(["Student", "AcademicStaff"])]
        public async Task<ActionResult<ItemReservation>> RequestReservation([FromBody] JsonElement jsonBody)
		{
            try {
                // Get the User from the token
                UserDTO? studentDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (studentDto == null) throw new Exception("Invalid Token/Authorization Header");
                User student = await _dbContext.users.Where(dbUser => dbUser.IsActive && dbUser.UserId == studentDto.UserId).FirstAsync();
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
                    StartDate = DateTime.Parse(reservationDTO.startDate),
                    EndDate = DateTime.Parse(reservationDTO.endDate),
                    ReservedUserId = studentDto.UserId,
                    ReservedUser = student,
                    CreatedAt = DateTime.Now,
                    Status = "Pending",
                    IsActive = true
                };
                await _dbContext.itemReservations.AddAsync(newItemReservation);
                await _dbContext.SaveChangesAsync();
                return StatusCode(201, newItemReservation);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
		}
        [HttpGet("StudentReservation")]
        [AuthorizationFilter(["Student"])]
        public async Task<ActionResult<ICollection<ItemReservation>>> ShowReservationList()
        {
            try
            {
                // Get the User from the token
                UserDTO? studentDto = await _tokenParser.getUser(HttpContext.Request.Headers["Authorization"].FirstOrDefault());
                if (studentDto == null) throw new Exception("Invalid Token/Authorization Header");
                User student = await _dbContext.users.Where(dbUser => dbUser.IsActive && dbUser.UserId == studentDto.UserId).FirstAsync();

                var reservationlist = student.ItemsReservedBy;
				return StatusCode(201, reservationlist); ;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
	}
}
