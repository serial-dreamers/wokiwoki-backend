using StackExchange.Redis;
using Wokiwoki.Application.Common.Interfaces.Services;

namespace Wokiwoki.Infrastructure.Services
{
	public class RedisCacheService : IRedisCacheService
	{
		private readonly IDatabase _database;

		public RedisCacheService(IConnectionMultiplexer connection)
		{
			_database = connection.GetDatabase();
		}

		public async Task SetAsync(string key, string value, TimeSpan expiry) 
			=> await _database.StringSetAsync(key, value, expiry);

		public async Task<string?> GetAsync(string key)
		{
			var val = await _database.StringGetAsync(key);
			return val.HasValue ? val.ToString() : null;
		}

		public async Task RemoveAsync(string key) => await _database.KeyDeleteAsync(key);
	}
}
