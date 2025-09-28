namespace Wokiwoki.Domain.Entities
{
	[Table("user_organization_follow")]
	public class UserOrganizationFollow : BaseAuditableEntity
	{
		public Guid UserId { get; set; }
		public Guid OrganizationId { get; set; }
		public Organization Organization { get; set; } = null!; 
	}
}
