using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using team4.LTS.Core.Model;
using team4.LTS.Core.Model.DTO;
using Team4.LTS.Infra.Data;

namespace team4.LTS.Service.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LabTrackerAdd : ControllerBase
	{
		private readonly DataBaseContext _context;

		public LabTrackerAdd(DataBaseContext context)
		{
			_context = context;
		}
		[HttpPost]
		[ProducesResponseType(201)]
		[ProducesResponseType(400)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> AddUser([FromBody] UserDTO userdto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var user = new User();
			user.Email = userdto.Email;
			user.FirstName = userdto.FirstName;
			user.LastName = userdto.LastName;
			user.Role = userdto.Role;
			user.IsActive = userdto.IsActive;

			_context.users.Add(user);
			await _context.SaveChangesAsync();

			return Created();
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<User>>> GetUsers()
		{
			return await _context.users.ToListAsync();
		}

		//admin: add new labs 
		[HttpPost]
		[ProducesResponseType(201)]
		[ProducesResponseType(400)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> AddLab([FromBody] LabDTO labdto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var lab = new Lab();
			lab.LabCode = labdto.LabCode;
			lab.ImageURL = labdto.ImageURL;
			lab.LabName= labdto.LabName;
			lab.IsActive = labdto.IsActive;

			_context.Labs.Add(lab);
			await _context.SaveChangesAsync();

			return Created();
		}
		//student: request equipment reservation 
		[HttpPost("{equipmentid:int},{reservedid:int}")]
		[ProducesResponseType(201)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> RequestEquipment([FromBody] int equipmentid, int reservedid)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
	        var reservation  = new ItemReservation();
			reservation.RequstedEquipmentId = equipmentid;
			reservation.ReservedBy = reservedid;
			reservation.CreatedAt = DateTime.UtcNow;
			_context.ItemReservations.Add(reservation);
			await _context.SaveChangesAsync();
			return Created();
		}
	}
}
