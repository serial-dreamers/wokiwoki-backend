using Medo;
using Wokiwoki.Application.Common.Interfaces.Services;

namespace Wokiwoki.Infrastructure.Services
{
	public class Uuid7GuidGenerator : IGuidGenerator
	{
		public Guid NewGuid() => Uuid7.NewUuid7();
	}
}
