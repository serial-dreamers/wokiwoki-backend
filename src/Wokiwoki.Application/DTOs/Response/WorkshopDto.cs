using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.DTOs.Response
{
	public class WorkshopDto
	{
		public string Title { get; set; } = null!;

		public string? ShortDescription { get; set; }

		public string Description { get; set; } = null!;

		public string ImageUrl { get; set; } = string.Empty;

		public decimal? DisplayPrice { get; set; }

		public string? DisplayLocation { get; set; }

		public double? Latitude { get; set; }

		public double? Longitude { get; set; }

		public int LikeCount { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public int Capacity { get; set; }

		public Guid OrganizationId { get; set; }

		public Organization Organization { get; set; } = null!;

		public Guid CategoryId { get; set; }

		public bool IsActive { get; set; } = true;

		public Category Category { get; set; } = null!;

		public Guid WorkshopTypeId { get; set; }

		public virtual WorkshopTypeDto WorkshopType { get; set; } = null!;

		public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>(); 

		public List<TagDto> Tags { get; set; } = new List<TagDto>();

		public List<WorkshopMediaDto> WorkshopMedias { get; set; } = new List<WorkshopMediaDto>();

		public List<WorkshopHeroMediaDto> WorkshopHeroMedias { get; set; } = new List<WorkshopHeroMediaDto>();
	}


}
