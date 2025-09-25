using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Domain.Entities
{
	[Table("WorkshopCategory")] 
	public class WorkshopCategory : BaseAuditableEntity
	{
		public string Name { get; set; } = null!;

		public string? Description { get; set; }

		public string? IconUrl { get; set; }

		public string? ImageUrl { get; set; }

		public ICollection<WorkshopTag> WorkshopTags { get; set; } = new List<WorkshopTag>();

		public ICollection<Workshop> Workshops { get; set; } = new List<Workshop>();
	}
}
