using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.DTOs
{
	public class SearchWorkshopDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string? ShortDescription { get; set; }
		public string ImageUrl { get; set; } = string.Empty;
		public DateTime StartTime { get; set; } 
		public DateTime EndTime { get; set; } 
		public WorkshopSession Session { get; set; }
		public WorkshopType WorkshopType { get; set; }
	}
}
