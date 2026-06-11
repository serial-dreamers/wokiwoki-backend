using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Admin.Queries.GetAdminDashboard
{
	public class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
	{
		private readonly IBookingRepository _bookingRepository;

		public GetAdminDashboardQueryHandler(IBookingRepository bookingRepository)
		{
			_bookingRepository = bookingRepository;
		}

		public async Task<AdminDashboardDto> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
		{
			return await _bookingRepository.GetAdminDashboardAsync(
				request.StartDate,
				request.EndDate,
				request.ChartGroupBy,
				cancellationToken);
		}
	}
}

