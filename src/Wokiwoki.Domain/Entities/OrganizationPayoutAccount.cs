using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Domain.Entities
{
	[Table("organization_payout_account")]
	public class OrganizationPayoutAccount : BaseAuditableEntity
	{
		public Guid OrganizationId { get; set; }

		public Organization Organization { get; set; } = null!;

		public string BankCode { get; set; } = null!;

		public string BankName { get; set; } = null!;

		public string AccountNumber { get; set; } = null!;

		public string AccountHolder { get; set; } = null!;

		public string? LogoUrl { get; set; }

		public string SwiftCode { get; set; } = null!;
	}
}
