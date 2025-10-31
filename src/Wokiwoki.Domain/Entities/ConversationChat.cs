namespace Wokiwoki.Domain.Entities
{
	[Table("conversationchat")]
	public class ConversationChat : BaseAuditableEntity
	{
		public string UserId { get; set; } = string.Empty;
		public string? Title { get; set; } 
		public bool IsActive { get; set; } = true;

		public virtual ICollection<MessageChat> MessagesChats { get; set; } = new List<MessageChat>();
	}
}
