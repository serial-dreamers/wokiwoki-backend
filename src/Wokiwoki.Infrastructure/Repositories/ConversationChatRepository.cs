using Wokiwoki.Application.Common.Interfaces.Repositories;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class ConversationChatRepository : BaseRepo<ConversationChat, Guid>, IConversationChatRepository
	{
		public ConversationChatRepository(WokiwokiDbContext context) : base(context)
		{
		}
	}
}
