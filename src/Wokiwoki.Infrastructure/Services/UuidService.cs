using Medo;
using Wokiwoki.Application.Common.Interfaces.Services;

namespace Wokiwoki.Infrastructure.Services
{
	public class UuidService : IUuidService
	{
		public Guid NewGuid() => Uuid7.NewUuid7();
	}
}
