namespace Wokiwoki.Domain.Entities
{
	[Table("user_organization_follow")]
	public class UserOrganizationFollow : BaseAuditableEntity
	{
		public string UserId { get; set; } = null!;
		public Guid OrganizationId { get; set; }
		public Organization Organization { get; set; } = null!; 
	}
}
