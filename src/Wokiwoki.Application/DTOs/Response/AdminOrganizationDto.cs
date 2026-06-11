namespace Wokiwoki.Application.DTOs.Response
{
	/// <summary>
	/// DTO for Admin Organization Management
	/// </summary>
	public class AdminOrganizationDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public string? Description { get; set; }
		public string? LogoUrl { get; set; }
		public string? ContactEmail { get; set; }
		public string? ContactPhone { get; set; }
		public string? Address { get; set; }
		public int FollowerCount { get; set; }
		public int Status { get; set; } // 0: Pending, 1: Accepted, 2: Suspended
		public string? Reason { get; set; } // Rejection/Suspension reason
		public string OwnerName { get; set; } = null!;
		public string OwnerEmail { get; set; } = null!;
		public int TotalWorkshops { get; set; }
		public DateTime Created { get; set; }
	}
}

