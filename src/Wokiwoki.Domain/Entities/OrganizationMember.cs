namespace Wokiwoki.Domain.Entities
{
	[Table("organization_member")]
	public class OrganizationMember : BaseAuditableEntity
	{
		public Guid UserId { get; set; }

		public string Role { get; set; } = null!;

		public DateTime JoinedAt { get; set; }

		public bool IsActive { get; set; } = true;

		public Guid OrganizationId { get; set; } 

		public Organization Organization { get; set; } = null!;
	}
}
