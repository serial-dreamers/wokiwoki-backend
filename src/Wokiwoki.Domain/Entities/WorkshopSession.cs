namespace Wokiwoki.Domain.Entities
{
	[Table("workshop_session")]
	public class WorkshopSession : BaseAuditableEntity
	{
		public string Title { get; set; } = null!;

		public string Description { get; set; } = null!;

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; } 

		public string Province { get; set; } = null!;

		public string District { get; set; } = null!;

		public string Ward { get; set; } = null!;

		public string AddressDetail { get; set; } = null!;

		public int Capacity { get; set; } 

		public Guid WorkshopId { get; set; }

		public Workshop Workshop { get; set; } = null!;

		public bool IsActive { get; set; } = true;

		public virtual ICollection<WorkshopTicketType> WorkshopTicketTypes { get; set; } = new List<WorkshopTicketType>();
	}
}
