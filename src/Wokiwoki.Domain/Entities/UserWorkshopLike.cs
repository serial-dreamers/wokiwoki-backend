namespace Wokiwoki.Domain.Entities
{
	[Table("user_workshop_like")]
	public class UserWorkshopLike : BaseAuditableEntity
	{
		public string UserId { get; set; } = null!;
		public Guid WorkshopId { get; set; }
		public Workshop Workshop { get; set; } = null!; 
	}
}
