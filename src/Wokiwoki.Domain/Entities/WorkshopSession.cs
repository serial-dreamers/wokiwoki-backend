using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Domain.Entities
{
	[Table("WorkshopSession")]
	public class WorkshopSession
	{
		public string Title { get; set; } = null!;

		public string Description { get; set; } = null!;

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public string? Location { get; set; }

		public int Capacity { get; set; }

		public decimal Price { get; set; }

		public Guid WorkshopId { get; set; }
			
		public Workshop Workshop { get; set; } = null!;

		public virtual ICollection<WorkshopTicketType> WorkshopTickets { get; set; } = new List<WorkshopTicketType>();
	}
}
