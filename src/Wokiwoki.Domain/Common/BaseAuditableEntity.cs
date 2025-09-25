using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Domain.Common
{
	public abstract class BaseAuditableEntity : BaseEntity
	{
		public DateTime Created { get; set; }

		public Guid? CreatedBy { get; set; }

		public DateTime LastModified { get; set; }

		public Guid? LastModifiedBy { get; set; }
	}
}
