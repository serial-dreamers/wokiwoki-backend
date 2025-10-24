namespace Wokiwoki.Application.Common.Interfaces.Services
{
	public interface IUserContext
	{
		bool IsAuthenticated { get; }
		string UserId { get; }
	}
}
