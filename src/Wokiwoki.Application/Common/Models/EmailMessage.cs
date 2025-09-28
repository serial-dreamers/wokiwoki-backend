using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Application.Common.Models
{
	public class EmailMessage
	{
		public string To { get; set; } = default!;
		public string Subject { get; set; } = default!;
		public string TemplateName { get; set; } = default!;
		public object TemplateData { get; set; } = default!;
	}
}
