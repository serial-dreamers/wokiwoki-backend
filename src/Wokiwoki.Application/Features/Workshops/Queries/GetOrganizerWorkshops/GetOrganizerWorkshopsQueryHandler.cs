using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Workshops.Queries.GetOrganizerWorkshops
{
	public class GetOrganizerWorkshopsQueryHandler : IRequestHandler<GetOrganizerWorkshopsQuery, List<WorkshopSimpleDto>>
	{
		private readonly IWorkshopRepository _workshopRepository;
		private readonly IUserContext _userContext;

		public GetOrganizerWorkshopsQueryHandler(
			IWorkshopRepository workshopRepository,
			IUserContext userContext)
		{
			_workshopRepository = workshopRepository;
			_userContext = userContext;
		}

		public async Task<List<WorkshopSimpleDto>> Handle(GetOrganizerWorkshopsQuery request, CancellationToken cancellationToken)
		{
			var userId = _userContext.UserId;
			if (string.IsNullOrEmpty(userId))
				throw new UnauthorizedAccessException("User not authenticated");

			return await _workshopRepository.GetOrganizerWorkshopsAsync(userId, cancellationToken);
		}
	}
}

