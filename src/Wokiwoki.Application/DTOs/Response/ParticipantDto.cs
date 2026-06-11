namespace Wokiwoki.Application.DTOs.Response
{
	/// <summary>
	/// DTO for displaying participant information (ticket + booking + workshop info)
	/// </summary>
	public class ParticipantDto
	{
		public Guid TicketId { get; set; }

		public Guid BookingId { get; set; }

		public string FullName { get; set; } = null!;

		public string PhoneNumber { get; set; } = null!;

		public int Quantity { get; set; }

		public string TicketTypeName { get; set; } = null!;

		public decimal Price { get; set; }

		public bool IsCheckedIn { get; set; }

		public DateTime? CheckedInAt { get; set; }

		public DateTime BookingDate { get; set; }

		// Workshop Info
		public Guid WorkshopId { get; set; }

		public string WorkshopTitle { get; set; } = null!;

		public string? WorkshopImageUrl { get; set; }

		// Session Info
		public Guid SessionId { get; set; }

		public string SessionTitle { get; set; } = null!;

		public DateTime SessionStartTime { get; set; }

		public DateTime? SessionEndTime { get; set; }

		public string? SessionLocation { get; set; }

		// Booking Status
		public int BookingStatus { get; set; }
	}

	/// <summary>
	/// Simple DTO for workshop list
	/// </summary>
	public class WorkshopSimpleDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = null!;
		public string? ImageUrl { get; set; }
	}

	/// <summary>
	/// Simple DTO for session list
	/// </summary>
	public class SessionSimpleDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = null!;
		public DateTime StartTime { get; set; }
		public DateTime? EndTime { get; set; }
	}
}

