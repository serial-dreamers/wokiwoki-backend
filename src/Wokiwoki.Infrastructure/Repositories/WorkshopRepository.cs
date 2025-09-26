using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class WorkshopRepository : BaseRepo<Workshop, Guid>, IWorkshopRepository
	{
		public WorkshopRepository(WokiwokiDbContext context) : base(context)
		{ 
		}
	}
}
