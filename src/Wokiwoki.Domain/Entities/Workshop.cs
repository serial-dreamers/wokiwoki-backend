using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Domain.Entities
{
	[Table("Workshops")]
	public class Workshop : BaseAuditableEntity
	{
		public string Title { get; set; } = null!;

		public string? ShortDescription { get; set; }  

		public string Description { get; set; } = null!;

		public string ImageUrl { get; set; } = string.Empty;

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public int Capacity { get; set; }

		public Guid OrganizationId { get; set; }

		public Organization Organization { get; set; } = null!;

		public Guid WorkshopCategoryId { get; set; }

		public WorkshopCategory WorkshopCategory { get; set; } = null!;

		public virtual ICollection<WorkshopSession> WorkshopSessions { get; set; } = new List<WorkshopSession>();

		public virtual ICollection<WorkshopTag> WorkshopTags { get; set; } = new List<WorkshopTag>();

		public ICollection<WorkshopMedia> WorkshopMedias { get; set; } = new List<WorkshopMedia>();

		public ICollection<WorkshopHeroMedia> WorkshopHeroMedias { get; set; } = new List<WorkshopHeroMedia>();
	}
}
