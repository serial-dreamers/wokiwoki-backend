namespace Wokiwoki.Domain.Entities
{
	[Table("user_workshop_like")]
	public class UserWorkshopLike : BaseAuditableEntity
	{
		public Guid UserId { get; set; }
		public Guid WorkshopId { get; set; }
		public Workshop Workshop { get; set; } = null!; 
	}
}
