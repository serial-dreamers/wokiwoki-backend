namespace Wokiwoki.Domain.Entities
{
	[Table("category")] 
	public class Category : BaseAuditableEntity
	{
		public string Name { get; set; } = null!;

		public string? Description { get; set; }

		public string? IconUrl { get; set; }

		public string? ImageUrl { get; set; }

		public bool IsActive { get; set; } = true;

		public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

		public virtual ICollection<Workshop> Workshops { get; set; } = new List<Workshop>();

		public virtual ICollection<UserTagPreference> TagPreferences { get; set; } = new List<UserTagPreference>();
	}
}
