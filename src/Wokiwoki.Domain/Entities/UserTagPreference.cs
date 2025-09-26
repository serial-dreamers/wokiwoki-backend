namespace Wokiwoki.Domain.Entities
{
	[Table("user_tag_preference")]
	public class UserTagPreference : BaseAuditableEntity
	{
		public Guid UserId { get; set; }

		public Guid TagId { get; set; }

		public Guid CategoryId { get; set; }

		public Category Category { get; set; } = null!;

		public Tag Tag { get; set; } = null!; 
	}
}
