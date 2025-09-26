using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Application.Common.Interfaces.Services
{
	public interface IGuidGenerator
	{
		Guid NewGuid();
	}
}
