namespace Wokiwoki.Application.DTOs.Response
{
	/// <summary>
	/// DTO for organizer dashboard statistics
	/// </summary>
	public class OrganizerDashboardDto
	{
		public DashboardSummaryDto Summary { get; set; } = new();
		public List<RevenueChartDataDto> RevenueChart { get; set; } = new();
		public List<WorkshopRevenueDetailDto> WorkshopDetails { get; set; } = new();
	}

	/// <summary>
	/// Summary statistics for dashboard
	/// </summary>
	public class DashboardSummaryDto
	{
		public int TotalWorkshops { get; set; }
		public int UpcomingWorkshops { get; set; }
		public int TotalTicketsSold { get; set; }
		public decimal TotalRevenue { get; set; }
		public decimal OrganizationRevenue { get; set; } // 95% of total (after 5% platform fee)
		public int TotalCheckedIn { get; set; }
		public int TotalPendingCheckIn { get; set; }
	}

	/// <summary>
	/// Revenue chart data for graphs
	/// </summary>
	public class RevenueChartDataDto
	{
		public string Date { get; set; } = string.Empty; // DD/MM or MM/YYYY format
		public decimal Revenue { get; set; }
		public decimal OrganizationRevenue { get; set; }
		public int TicketsSold { get; set; }
	}

	/// <summary>
	/// Workshop revenue details for table
	/// </summary>
	public class WorkshopRevenueDetailDto
	{
		public Guid WorkshopId { get; set; }
		public string WorkshopTitle { get; set; } = string.Empty;
		public string? WorkshopImageUrl { get; set; }
		public int TotalSessions { get; set; }
		public int TotalTicketsSold { get; set; }
		public decimal TotalRevenue { get; set; }
		public decimal OrganizationRevenue { get; set; }
		public int TicketsCheckedIn { get; set; }
		public int TicketsPendingCheckIn { get; set; }
		public DateTime? LatestSessionDate { get; set; }
	}
}

