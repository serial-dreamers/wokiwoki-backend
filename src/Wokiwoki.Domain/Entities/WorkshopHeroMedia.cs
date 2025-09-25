using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Domain.Entities
{
	[Table("WorkshopHeroMedia")]
	public class WorkshopHeroMedia : BaseAuditableEntity
	{
		public HeroMediaType HeroType { get; set; }  

		public Guid? GalleryId { get; set; }

		public Guid WorkshopId { get; set; }

		public Workshop Workshop { get; set; } = null!;

		public WorkshopMedia Gallery { get; set; } = null!;
	}
}
