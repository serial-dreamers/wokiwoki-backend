namespace Wokiwoki.Domain.Entities
{
	[Table("workshop")]
	public class Workshop : BaseAuditableEntity
	{
		public string Title { get; set; } = null!;

		public string Summary { get; set; } = null!;

		public string Description { get; set; } = null!;	
		
		public string ImageUrl { get; set; } = string.Empty;

		public string? DisplayAddress { get; set; }  
		public double? Latitude { get; set; }
		public double? Longitude { get; set; }

		// Online (nếu online/hybrid)
		public string? OnlineEventUrl { get; set; }

		public int? DurationMinutes { get; set; }
		public int DefaultCapacity { get; set; }

		public int LikeCount { get; set; }
		public int TotalBookings { get; set; }
		public int ReviewCount { get; set; }
		public double AverageRating { get; set; }

		public bool UsesDefaultPricing { get; set; }

		public RefundPolicyType RefundPolicy { get; set; }
		public int? RegistrationDeadlineHours { get; set; }
		public string? RefundPolicyDescription { get; set; }

		public decimal? StartingPrice { get; set; }
		/// <summary>
		/// Trạng thái của workshop: Draft, PendingReview, Published, Hidden, Cancelled
		/// </summary>
		public WorkshopStatus Status { get; set; }
		public WorkshopDeliveryType DeliveryType { get; set; }  // Online, Offline, Hybrid
		public WorkshopScheduleType ScheduleType { get; set; }  // Recurring, OneTime 

		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }

		public Guid OrganizationId { get; set; }

		public Guid CategoryId { get; set; }  

		public bool IsActive { get; set; } = true;

		public Category Category { get; set; } = null!; 

		public Organization Organization { get; set; } = null!;

		public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

		public virtual ICollection<UserWorkshopLike> Likes { get; set; } = new List<UserWorkshopLike>();

		public ICollection<WorkshopSchedule> Schedules { get; set; } = new List<WorkshopSchedule>();

		public virtual ICollection<WorkshopSession> WorkshopSessions { get; set; } = new List<WorkshopSession>();

		public ICollection<WorkshopScheduleTicket> ScheduleTickets { get; set; } = new List<WorkshopScheduleTicket>();

		public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

		public virtual ICollection<WorkshopMedia> WorkshopMedias { get; set; } = new List<WorkshopMedia>();

		public virtual ICollection<WorkshopHeroMedia> WorkshopHeroMedias { get; set; } = new List<WorkshopHeroMedia>();
	}
}
