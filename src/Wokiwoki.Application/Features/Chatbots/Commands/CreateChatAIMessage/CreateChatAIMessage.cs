using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.DTOs;

namespace Wokiwoki.Application.Features.Chatbots.Commands.CreateChatAIMessage
{
	public record CreateChatAIMessageCommand(
		string userMessage, string userId) : IRequest<ChatResponse>;

	public class CreateChatAIMessageCommandHandler : IRequestHandler<CreateChatAIMessageCommand, ChatResponse>
	{
		private readonly IAzureOpenAIChatService _azureOpenAIChatService;
		public CreateChatAIMessageCommandHandler(IAzureOpenAIChatService azureOpenAIChatService)
		{
			_azureOpenAIChatService = azureOpenAIChatService;
		}
		public async Task<ChatResponse> Handle(CreateChatAIMessageCommand request, CancellationToken cancellationToken)
		{
			var response = await _azureOpenAIChatService.ChatAsync(request.userMessage, request.userId);
			return response;
		}
	}
}
