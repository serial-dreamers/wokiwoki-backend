using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Domain.Entities
{
	[Table("WorkshopType")]	
	public class WorkshopType : BaseAuditableEntity
	{
		public string Name { get; set; } = null!;

		public string? Description { get; set; }

		public string? IconUrl { get; set; } 

		public virtual ICollection<Workshop> Workshops { get; set; } = new List<Workshop>();
	}
}
