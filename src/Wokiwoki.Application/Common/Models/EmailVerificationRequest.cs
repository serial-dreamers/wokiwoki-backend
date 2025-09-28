using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Application.Common.Models
{
	public class EmailVerificationRequest
	{
		public string To { get; set; } = default!;

		public string Subject { get; set; } = "Email Verification";

		public string Code { get; set; } = default!;
	}
}
