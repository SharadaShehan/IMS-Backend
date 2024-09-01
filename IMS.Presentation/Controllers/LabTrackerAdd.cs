﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IMS.ApplicationCore.Model;
using IMS.ApplicationCore.Model.DTO;
using IMS.Infrastructure.Services;

namespace IMS.Presentation.Controllers
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
		[HttpPost("AddUser")]
		[ProducesResponseType(201)]
		[ProducesResponseType(204)]
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

		//[HttpGet]
		//public async Task<ActionResult<IEnumerable<User>>> GetUsers()
		//{
		//	return await _context.users.ToListAsync();
		//}

		//admin: add new labs
		[HttpPost("AddNewLab")]
		[ProducesResponseType(201)]
		[ProducesResponseType(400)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> AddNewLab([FromBody] LabDTO labdto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var lab = new Lab();
			lab.LabCode = labdto.LabCode;
			lab.ImageURL = labdto.ImageURL;
			lab.LabName = labdto.LabName;
			lab.IsActive = labdto.IsActive;

			_context.Labs.Add(lab);
			await _context.SaveChangesAsync();

			return Created();
		}


		//student: request equipment reservation 
		[HttpPost("RequestEquipment")]
		[ProducesResponseType(201)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> RequestEquipment([FromBody] ReservetionRequestDTO requestdto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
	        var reservation  = new ItemReservation();
			reservation.RequstedEquipmentId = requestdto.RequstedEquipmentId;
			reservation.ReservedBy = requestdto.ReservedBy;
			reservation.CreatedAt = DateTime.UtcNow;
			reservation.FromDate = DateTime.UtcNow.AddDays(3);
			reservation.ToDate = DateTime.UtcNow.AddDays(17);
			reservation.Status = "Pending";
			_context.ItemReservations.Add(reservation);
			await _context.SaveChangesAsync();
			return Created();
		}
	}
}