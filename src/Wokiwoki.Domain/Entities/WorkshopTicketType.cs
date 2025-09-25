using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Domain.Entities
{
	[Table("WorkshopTicketType")]
	public class WorkshopTicketType : BaseAuditableEntity
	{
		public string Name { get; set; }= null!;

		public string? Description { get; set; }

		public decimal Price { get; set; }

		public int Quantity { get; set; }

		public int Sold { get; set; }

		public DateTime SaleStart { get; set; }

		public DateTime SaleEnd { get; set; }

		public Guid WorkshopSessionId { get; set; }

		public WorkshopSession Session { get; set; } = null!;
	}
}
