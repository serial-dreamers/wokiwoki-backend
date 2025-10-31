using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.DTOs.Response
{
	public class WorkshopHeroMediaDto
	{
		public Guid? Id { get; set; }

		public HeroMediaType HeroType { get; set; }

		public Guid? MediaId { get; set; }

		public Guid WorkshopId { get; set; } 

	}
}
