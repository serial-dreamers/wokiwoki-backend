using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.DTOs.Response
{
	/// <summary>
	/// DTO tối ưu cho Calendar view - chỉ chứa thông tin cần thiết
	/// </summary>
	public class WorkshopSessionCalendarDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = null!;
		public string Description { get; set; } = null!;
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public int Capacity { get; set; }
		public int BookedCount { get; set; }

		// Workshop info
		public Guid WorkshopId { get; set; }
		public string WorkshopTitle { get; set; } = null!;
		public string WorkshopImageUrl { get; set; } = string.Empty;
		public WorkshopStatus WorkshopStatus { get; set; }
		public WorkshopDeliveryType DeliveryType { get; set; }
		
		// Location
		public string? DisplayAddress { get; set; }
		public string? OnlineEventUrl { get; set; }
		
		// Pricing
		public decimal? StartingPrice { get; set; }
	}
}

