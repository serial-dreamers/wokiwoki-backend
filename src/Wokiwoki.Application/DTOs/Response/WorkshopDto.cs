using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.DTOs.Response
{
	public class WorkshopDto
	{
		public Guid Id { get; set; }

		public string Title { get; set; } = null!;

		public string Summary { get; set; } = null!;

		public string Description { get; set; } = null!;

		public string ImageUrl { get; set; } = string.Empty;

		public string? DisplayAddress { get; set; }

		public double? Latitude { get; set; }

		public double? Longitude { get; set; }

		public string? OnlineEventUrl { get; set; }

		public int? DurationMinutes { get; set; }

		public int DefaultCapacity { get; set; }

		public int LikeCount { get; set; }

		public int TotalBookings { get; set; }

		public int ReviewCount { get; set; }

		public double AverageRating { get; set; }

		public RefundPolicyType RefundPolicy { get; set; }

		public int? RegistrationDeadlineHours { get; set; }

		public string? RefundPolicyDescription { get; set; }

		public decimal? StartingPrice { get; set; }

		public WorkshopStatus Status { get; set; }

		public WorkshopDeliveryType DeliveryType { get; set; }

		public WorkshopScheduleType ScheduleType { get; set; }

		public Guid OrganizationId { get; set; }

		public Guid CategoryId { get; set; }

		public bool IsActive { get; set; } = true;
		 
		public CategoryDto? Category { get; set; }

		public OrganizationDto? Organization { get; set; }

		public List<ReviewDto> Reviews { get; set; } = new();

		public List<TagDto> Tags { get; set; } = new();

		public List<WorkshopMediaDto> WorkshopMedias { get; set; } = new();

		public List<WorkshopHeroMediaDto> WorkshopHeroMedias { get; set; } = new();
	}


	public class DiscoverWorkshopDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = null!;
		public string Summary { get; set; } = null!;
		public string ImageUrl { get; set; } = string.Empty;
		public string? DisplayAddress { get; set; }
		public decimal? StartingPrice { get; set; }
		public int LikeCount { get; set; }
		public int ReviewCount { get; set; }
		public double AverageRating { get; set; }
		public WorkshopDeliveryType DeliveryType { get; set; }

		// Organization info for discover section
		public DiscoverOrganizationDto? Organization { get; set; }
	}

	public class DiscoverOrganizationDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? LogoUrl { get; set; }
		public string? Description { get; set; }
	}


}
