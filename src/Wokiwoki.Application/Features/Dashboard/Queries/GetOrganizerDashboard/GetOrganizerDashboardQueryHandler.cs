using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Dashboard.Queries.GetOrganizerDashboard
{
	public class GetOrganizerDashboardQueryHandler : IRequestHandler<GetOrganizerDashboardQuery, OrganizerDashboardDto>
	{
		private readonly IBookingRepository _bookingRepository;
		private readonly IUserContext _userContext;

		public GetOrganizerDashboardQueryHandler(
			IBookingRepository bookingRepository,
			IUserContext userContext)
		{
			_bookingRepository = bookingRepository;
			_userContext = userContext;
		}

		public async Task<OrganizerDashboardDto> Handle(GetOrganizerDashboardQuery request, CancellationToken cancellationToken)
		{
			// Get current user ID
			var userId = _userContext.UserId;
			if (string.IsNullOrEmpty(userId))
				throw new UnauthorizedAccessException("User not authenticated");

			// Use repository method to get dashboard data
			return await _bookingRepository.GetOrganizerDashboardAsync(
				userId,
				request.StartDate,
				request.EndDate,
				request.WorkshopId,
				request.ChartGroupBy,
				cancellationToken
			);
		}
	}
}

