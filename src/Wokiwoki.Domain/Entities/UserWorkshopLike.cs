using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Domain.Entities
{
	[Table("UserWorkshopLike")]
	public class UserWorkshopLike : BaseAuditableEntity
	{
		public Guid UserId { get; set; }
		public Guid WorkshopId { get; set; }
		public Workshop Workshop { get; set; } = null!;
		public DateTime LikedAt { get; set; }
	}
}
