using Microsoft.EntityFrameworkCore;
using Wokiwoki.Application.Common.Interfaces.Repositories;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class UserWorkshopLikeRepository : BaseRepo<UserWorkshopLike, Guid>, IUserWorkshopLikeRepository
	{ 

		public UserWorkshopLikeRepository(WokiwokiDbContext context) : base(context)
		{
			
		} 
		public async Task<bool> ExistsAsync(string userId, Guid workshopId, CancellationToken cancellationToken)
		{
			return await _context.UserWorkshopLikes
		   .AnyAsync(x => x.UserId == userId && x.WorkshopId == workshopId, cancellationToken);
		}

		public async Task<UserWorkshopLike?> GetExistsTAsync(string userId, Guid workshopId, CancellationToken cancellationToken)
		=> await _context.UserWorkshopLikes
	   .FirstOrDefaultAsync(x => x.UserId == userId && x.WorkshopId == workshopId, cancellationToken);
		
		
	}
}
