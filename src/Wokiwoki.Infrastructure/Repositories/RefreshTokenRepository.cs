using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class RefreshTokenRepository : BaseRepo<RefreshToken, Guid>, IRefreshTokenRepository
	{
		public RefreshTokenRepository(WokiwokiDbContext context) : base(context)
		{
		}
		public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
		{
			_context.RefreshTokens.Add(refreshToken);
			await _context.SaveChangesAsync();
		}

		public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
		{
			return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
		}
	}
}
