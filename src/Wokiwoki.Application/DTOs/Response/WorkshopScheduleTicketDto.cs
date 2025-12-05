using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Application.DTOs.Response
{
	public class WorkshopScheduleTicketDto
	{
		public Guid Id { get; set; }
		public Guid WorkshopScheduleId { get; set; } 
		public string Name { get; set; } = null!;  
		public decimal Price { get; set; }
		public int MaxQuantity { get; set; } = 20;
		public bool IsActive { get; set; } = true;
	}
}
