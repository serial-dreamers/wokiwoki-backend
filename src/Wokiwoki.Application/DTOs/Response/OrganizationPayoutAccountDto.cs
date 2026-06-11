namespace Wokiwoki.Application.DTOs.Response
{
	public class OrganizationPayoutAccountDto
	{
		public Guid Id { get; set; }
		public Guid OrganizationId { get; set; }
		public string BankCode { get; set; } = null!;
		public string BankName { get; set; } = null!;
		public string AccountNumber { get; set; } = null!;
		public string AccountHolder { get; set; } = null!;
		public string? LogoUrl { get; set; }
		public string SwiftCode { get; set; } = null!;
		public DateTime Created { get; set; }
	}
}

