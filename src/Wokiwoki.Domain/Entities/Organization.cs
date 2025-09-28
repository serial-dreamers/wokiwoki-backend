namespace Wokiwoki.Domain.Entities
{
	[Table("organization")]
	public class Organization : BaseAuditableEntity
	{
		public string Name { get; set; } = null!;

		public string? Description { get; set; }

		public string? LogoUrl { get; set; }

		public int FollowerCount { get; set; } 

		public string? ContactEmail { get; set; }

		public string? ContactPhone { get; set; }

		public string? Street { get; set; }

		public string? Ward { get; set; }

		public string? District { get; set; }

		public string? Province { get; set; }

		public bool IsActive { get; set; } = true;

		public ICollection<UserOrganizationFollow> Followers { get; set; } = new List<UserOrganizationFollow>();

		public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = new List<OrganizationMember>();
	}
}
