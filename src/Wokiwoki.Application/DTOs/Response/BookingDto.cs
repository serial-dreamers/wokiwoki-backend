using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.DTOs.Response
{
	public class BookingDto
	{
		public Guid Id { get; set; }

		public string UserId { get; set; }

		public Guid WorkshopId { get; set; }

		public decimal TotalPrice { get; set; }

		public BookingStatus Status { get; set; }

		public string? FullName { get; set; }

		public string? PhoneNumber { get; set; }

		public WorkshopDto Workshop { get; set; } = null!;

		public bool IsActive { get; set; } = true;

		public DateTime Created { get; set; }

	}
}
