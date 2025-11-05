namespace Wokiwoki.Application.DTOs
{
	public class GoongAutocompleteResponse
	{
		public List<Prediction>? Predictions { get; set; }
		public string? Status { get; set; }
	}

	public class Prediction
	{
		public string? Description { get; set; }
		public string? PlaceId { get; set; }
	}

	public class GoongPlaceSuggestion
	{
		public string Description { get; set; } = string.Empty;
		public string PlaceId { get; set; } = string.Empty;
	}
}

