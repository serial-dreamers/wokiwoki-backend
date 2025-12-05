using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.DTOs.Response
{
	public class WorkshopSessionDto
	{
		public Guid Id { get; set; }
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
		public Guid WorkshopId { get; set; }
		public Guid? ScheduleId { get; set; }
		public bool IsActive { get; set; } = true;

	}
}
