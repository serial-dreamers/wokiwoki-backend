namespace Wokiwoki.Application.DTOs.Response
{
	/// <summary>
	/// DTO for Admin User Management
	/// </summary>
	public class AdminUserDto
	{
		public string Id { get; set; } = null!;
		public string? FullName { get; set; }
		public string Email { get; set; } = null!;
		public string? PhoneNumber { get; set; }
		public string? AvatarUrl { get; set; }
		public List<string> Roles { get; set; } = new List<string>();
		public bool IsEmailConfirmed { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? LastLoginAt { get; set; }
		public int TotalBookings { get; set; }
		public decimal TotalSpent { get; set; }
	}
}

