using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.ApplicationCore.Model.DTO
{
	public  class ReservetionRequestDTO
	{
		[Required]
		public int RequstedEquipmentId { get; set; }
		[Required]
		public int ReservedBy { get; set; }

	}
}
