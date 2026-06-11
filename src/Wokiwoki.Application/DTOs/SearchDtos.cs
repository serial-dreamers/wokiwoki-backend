using System.ComponentModel.DataAnnotations;
namespace Wokiwoki.Application.DTOs
{
	public class SearchWorkshopArgs
	{
		public string? Keyword { get; set; }
		public string? CategoryName { get; set; }
		public List<string>? TagNames { get; set; }
		public string? DeliveryType { get; set; }
		public string? Location { get; set; }
		public decimal? MaxPrice { get; set; }
		public decimal? MinPrice { get; set; }
		public double? MinRating { get; set; }  // 1-5
		public int? Offset { get; set; } = 0;
		public int? Limit { get; set; } = 5;
	}

	public class SearchOrgArgs
	{
		public string? Name { get; set; }
		public string? Location { get; set; }
	}

	public class GetDetailsArgs
	{
		[Required]
		public string WorkshopId { get; set; } = string.Empty;
	}

	public class SearchByTagsArgs
	{
		[Required]
		[MinLength(1)]
		public List<string> TagNames { get; set; } = new();

		public int? Offset { get; set; } = 0;
	}

	public class SemanticSearchArgs
	{
		[Required]
		public string Query { get; set; } = string.Empty;
		public int Limit { get; set; } = 10;
	}
}
