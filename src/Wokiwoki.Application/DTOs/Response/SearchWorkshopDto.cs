namespace Wokiwoki.Application.DTOs.Response
{
	public class SearchWorkshopDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = null!;
		public string Summary { get; set; } = null!; 
		public string ImageUrl { get; set; } = string.Empty;
		public decimal? StartingPrice { get; set; }
		public DateTime StartTime { get; set; } 
		public DateTime EndTime { get; set; } 
		public double? Latitude { get; set; } // Workshop level coordinates (priority)
		public double? Longitude { get; set; } // Workshop level coordinates (priority)
		public string? DisplayAddress { get; set; } // Workshop level address
		public WorkshopSessionDto? Session { get; set; } 
	}
}
