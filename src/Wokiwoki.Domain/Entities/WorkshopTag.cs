using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Domain.Entities
{
	[Table("WorkshopTag")]
	public class WorkshopTag : BaseAuditableEntity
	{ 
		public string? Name { get; set; }

		public string? Description { get; set; }

		public string? IconUrl { get; set; }

		public Guid WorkshopCategoryId { get; set; }

		public WorkshopCategory Category { get; set; } = null!;

		public virtual ICollection<Workshop> Workshops { get; set; } = new List<Workshop>();
	}
}
