using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Domain.Entities
{
	[Table("WorkshopGallery")]
	public class WorkshopMedia : BaseAuditableEntity
	{
		public string? ImageUrl { get; set; }

		public GalleryType MediaType { get; set; }

		public DateTime UploadedAt { get; set; }

		public Guid UploadedByUserId { get; set; }

		public Guid WorkshopId { get; set; }

		public Workshop Workshop { get; set; }
	}
}
