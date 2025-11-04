namespace Wokiwoki.Application.Common.Interfaces.Services
{
	public interface IGoongMapService
	{
		Task<(double lat, double lng)?> GetCoordinatesAsync(string address);
	}
}
