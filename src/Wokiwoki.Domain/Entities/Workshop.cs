namespace Wokiwoki.Domain.Entities
{
	[Table("workshop")]
	public class Workshop : BaseAuditableEntity
	{
		public string Title { get; set; } = null!;

		public string? ShortDescription { get; set; }  

		public string Description { get; set; } = null!;

		public string ImageUrl { get; set; } = string.Empty;

		public int LikeCount { get; set; } 

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public int Capacity { get; set; }

		public Guid OrganizationId { get; set; }

		public Organization Organization { get; set; } = null!;

		public Guid CategoryId { get; set; }

		public bool IsActive { get; set; } = true;

		public Category Category { get; set; } = null!;

		public virtual ICollection<UserWorkshopLike> Likes { get; set; } = new List<UserWorkshopLike>();

		public virtual ICollection<WorkshopSession> WorkshopSessions { get; set; } = new List<WorkshopSession>();

		public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

		public virtual ICollection<WorkshopMedia> WorkshopMedias { get; set; } = new List<WorkshopMedia>();

		public virtual ICollection<WorkshopHeroMedia> WorkshopHeroMedias { get; set; } = new List<WorkshopHeroMedia>();
	}
}
