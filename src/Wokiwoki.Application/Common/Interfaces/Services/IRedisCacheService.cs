using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Application.Common.Interfaces.Services
{
	public interface IRedisCacheService
	{
		Task SetAsync(string key, string value, TimeSpan expiry);

		Task<string?> GetAsync(string key);

		Task RemoveAsync(string key);
	}
}
