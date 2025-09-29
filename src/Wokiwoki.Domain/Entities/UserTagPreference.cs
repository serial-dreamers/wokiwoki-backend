namespace Wokiwoki.Domain.Entities
{
	[Table("user_tag_preference")]
	public class UserTagPreference : BaseAuditableEntity
	{
		public string UserId { get; set; } = null!;

		public Guid TagId { get; set; }

		public Guid CategoryId { get; set; }

		public Category Category { get; set; } = null!;

		public Tag Tag { get; set; } = null!; 
	}
}
