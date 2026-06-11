using MediatR;
using System.Linq;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.WorkshopSessions.Queries.GetSessionsByWorkshop
{
	public class GetSessionsByWorkshopQueryHandler : IRequestHandler<GetSessionsByWorkshopQuery, List<SessionSimpleDto>>
	{
		private readonly IWorkshopSessionRepository _workshopSessionRepository;

		public GetSessionsByWorkshopQueryHandler(IWorkshopSessionRepository workshopSessionRepository)
		{
			_workshopSessionRepository = workshopSessionRepository;
		}

		public async Task<List<SessionSimpleDto>> Handle(GetSessionsByWorkshopQuery request, CancellationToken cancellationToken)
		{
			var sessions = await _workshopSessionRepository.GetSessionsByWorkshopId(request.WorkshopId, cancellationToken);

			return sessions.Select(s => new SessionSimpleDto
			{
				Id = s.Id,
				Title = s.Title,
				StartTime = s.StartTime,
				EndTime = s.EndTime
			}).ToList();
		}
	}
}

