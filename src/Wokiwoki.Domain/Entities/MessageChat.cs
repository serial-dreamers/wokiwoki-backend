namespace Wokiwoki.Domain.Entities
{
	[Table("messagechat")]
	public class MessageChat : BaseAuditableEntity
	{
		public Guid ConversationId { get; set; }
		public string Role { get; set; } = string.Empty;  
		public string Content { get; set; } = string.Empty; 

		// Navigation
		public virtual ConversationChat ConversationChat { get; set; } = null!;
	}
}
