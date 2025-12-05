using Wokiwoki.Application.DTOs;

namespace Wokiwoki.Application.Common.Interfaces.Services
{
	public interface IGoongMapService
	{
		Task<(double lat, double lng)?> GetCoordinatesAsync(string address);
		Task<List<GoongPlaceSuggestion>> GetPlaceSuggestionsAsync(string input, CancellationToken cancellationToken = default);
	}
}
