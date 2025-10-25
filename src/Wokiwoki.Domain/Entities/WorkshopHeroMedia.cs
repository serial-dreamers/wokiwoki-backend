namespace Wokiwoki.Domain.Entities
{
	[Table("workshop_hero_media")]
	public class WorkshopHeroMedia : BaseAuditableEntity
	{
		public HeroMediaType HeroType { get; set; }  

		public Guid? MediaId { get; set; }

		public Guid WorkshopId { get; set; }

		public bool IsActive { get; set; } = true;

		public Workshop Workshop { get; set; } = null!;

		public WorkshopMedia WorkshopMedia { get; set; } = null!;
	}
}
