using Wokiwoki.Application.DTOs;

namespace Wokiwoki.Application.Common.Interfaces.Services
{
	public interface IAzureOpenAIChatService
	{
		Task<ChatResponse> ChatAsync(string userMessage, string? userId);

	}
}
