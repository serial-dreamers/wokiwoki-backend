namespace Wokiwoki.Domain.Entities
{
	[Table("tag")]
	public class Tag : BaseAuditableEntity
	{ 
		public string? Name { get; set; }

		public string? Description { get; set; }

		public string? IconUrl { get; set; }

		public bool IsActive { get; set; } = true;

		public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

		public virtual ICollection<Workshop> Workshops { get; set; } = new List<Workshop>();

		public ICollection<UserTagPreference> UserPreferences { get; set; } = new List<UserTagPreference>();
	}
}
