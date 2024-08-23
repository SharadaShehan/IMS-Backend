using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace team4.LTS.Core.Model.DTO
{
	public class MaintenanceDTO
	{
		public int MaintenanceId { get; set; }
		public int ItemId { get; set; }
		public int AssignedTechnician { get; set; }
		public int AssignedBy { get; set; }
		public int ReviwedBy { get; set; }
		public string? TaskDescription { get; set; }
		public string? SubmitNote { get; set; }
		public string? ReviewNote { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int Cost { get; set; }
		[RegularExpression(@"^(Unassigned|Ongoin|UnderReview|Completed)$", ErrorMessage = "ADD ERROR MESSAGE")]
		public string? Status { get; set; }
		public DateTime CeatedAt { get; set; }
		public DateTime RepairedAt { get; set; }
		public DateTime ReviewedAT { get; set; }
		public Boolean IsActive { get; set; }
	}
}
