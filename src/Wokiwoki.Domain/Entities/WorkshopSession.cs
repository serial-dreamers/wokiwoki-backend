namespace Wokiwoki.Domain.Entities
{
	[Table("workshop_session")]
	public class WorkshopSession : BaseAuditableEntity
	{
		public string Title { get; set; } = null!;

		public string Description { get; set; } = null!;

		public DateTime StartTime { get; set; } 
		public DateTime EndTime { get; set; }

		public string? Street { get; set; }
		public string? Commune { get; set; }
		public string? Province { get; set; }
		public double? Latitude { get; set; }
		public double? Longitude { get; set; }

		public AgeRestrictionType AgeRestrictionType { get; set; }
		public int? MinimumAge { get; set; }

		public ParkingType? ParkingType { get; set; }
		public string? ParkingDescription { get; set; }

		public int Capacity { get; set; }
		public int BookedCount { get; set; }
		public bool HasCustomPricing { get; set; }

		public Guid WorkshopId { get; set; }
		public Guid? ScheduleId { get; set; } // null = session riêng, không theo định kỳ

		public bool IsActive { get; set; } = true;

		public Workshop Workshop { get; set; } = null!;

		public virtual ICollection<WorkshopSessionTicket> WorkshopSessionTickets { get; set; } = new List<WorkshopSessionTicket>();
	}
}
