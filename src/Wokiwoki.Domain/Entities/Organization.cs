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

		public string? Commune { get; set; }

		public string? Province { get; set; }

		public string OwnerId { get; set; } = null!;
		 
		public OrganizationStatus Status { get; set; } 

		public bool IsActive { get; set; } = true;

		public ICollection<UserOrganizationFollow> Followers { get; set; } = new List<UserOrganizationFollow>();

		public OrganizationPayoutAccount? PayoutAccount { get; set; }
	}
}
