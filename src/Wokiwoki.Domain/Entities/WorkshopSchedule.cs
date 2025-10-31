namespace Wokiwoki.Domain.Entities
{
	[Table("workshop_schedule")]
	public class WorkshopSchedule : BaseAuditableEntity
	{
		public Guid WorkshopId { get; set; }

		public RecurrenceType RecurrenceType { get; set; }

		public string? DaysOfWeek { get; set; }
		public string? DaysOfMonth { get; set; }

		public TimeOnly StartTime { get; set; }
		public TimeOnly EndTime { get; set; }

		public DateTime ValidFrom { get; set; }
		public DateTime? ValidUntil { get; set; }

		public int? Capacity { get; set; }

		public Workshop Workshop { get; set; } = null!;
		public ICollection<WorkshopSession> Sessions { get; set; } = new List<WorkshopSession>();
        public ICollection<WorkshopScheduleTicket> Tickets { get; set; } = new List<WorkshopScheduleTicket>();
    }
}
