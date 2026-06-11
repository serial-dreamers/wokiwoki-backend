namespace Wokiwoki.Application.DTOs.Response
{
	/// <summary>
	/// DTO for Admin Dashboard statistics
	/// </summary>
	public class AdminDashboardDto
	{
		public AdminDashboardSummaryDto Summary { get; set; } = new AdminDashboardSummaryDto();
		public List<AdminRevenueChartDataDto> RevenueChart { get; set; } = new List<AdminRevenueChartDataDto>();
		public List<AdminWorkshopStatDto> TopWorkshops { get; set; } = new List<AdminWorkshopStatDto>();
	}

	public class AdminDashboardSummaryDto
	{
		public int TotalUsers { get; set; }
		public int TotalOrganizations { get; set; }
		public int PendingOrganizations { get; set; }
		public int TotalWorkshops { get; set; }
		public int PendingWorkshops { get; set; }
		public int TotalTicketsSold { get; set; }
		public decimal TotalRevenue { get; set; }
		public decimal PlatformRevenue { get; set; } // 5% platform fee
	}

	public class AdminRevenueChartDataDto
	{
		public string Date { get; set; } = null!; // e.g., "dd/MM/yyyy", "Wxx yyyy", "MM/yyyy"
		public decimal TotalRevenue { get; set; }
		public decimal PlatformRevenue { get; set; } // 5% of total
		public int TicketsSold { get; set; }
		public int WorkshopsCount { get; set; }
	}

	public class AdminWorkshopStatDto
	{
		public Guid WorkshopId { get; set; }
		public string WorkshopTitle { get; set; } = null!;
		public string? WorkshopImageUrl { get; set; }
		public string OrganizationName { get; set; } = null!;
		public int TotalTicketsSold { get; set; }
		public decimal TotalRevenue { get; set; }
		public decimal PlatformRevenue { get; set; } // 5%
		public int Status { get; set; }
	}
}

