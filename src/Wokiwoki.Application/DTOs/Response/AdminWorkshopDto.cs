namespace Wokiwoki.Application.DTOs.Response
{
	/// <summary>
	/// DTO for Admin Workshop Management
	/// </summary>
	public class AdminWorkshopDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = null!;
		public string? Summary { get; set; }
		public string? ImageUrl { get; set; }
		public int Status { get; set; } // 0: Draft, 1: PendingReview, 2: Published, 3: Hidden, 4: Cancelled
		public string? Reason { get; set; } // Rejection/Cancellation reason
		public string OrganizationName { get; set; } = null!;
		public Guid OrganizationId { get; set; }
		public string CategoryName { get; set; } = null!;
		public int TotalBookings { get; set; }
		public int TotalTicketsSold { get; set; }
		public decimal? TotalRevenue { get; set; }
		public DateTime Created { get; set; }
		public DateTime? LastModified { get; set; }
	}
}

