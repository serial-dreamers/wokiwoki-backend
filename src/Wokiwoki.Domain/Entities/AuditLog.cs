using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Domain.Entities
{
	[Table("AuditLog")]
	public class AuditLog : BaseAuditableEntity
	{ 
		public string? Action { get; set; }  // Create, Update, Delete

		public string? EntityName { get; set; }

		public Guid? EntityId { get; set; }

		public string? OriginalValue { get; set; }

		public string? NewValue { get; set; } 

		public DateTime PerformedAt { get; set; }
	}
}
