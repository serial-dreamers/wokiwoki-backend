using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Domain.Entities
{
	[Table("OrganizationMember")]
	public class OrganizationMember : BaseAuditableEntity
	{

		public string Role { get; set; } = null!;

		public DateTime JoinedAt { get; set; }

		public bool IsActive { get; set; }

		public Guid OrganizationId { get; set; }

		public Organization Organization { get; set; } = null!;
	}
}
