using MediatR;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Admin.Queries.GetAdminDashboard
{
	public record GetAdminDashboardQuery(
		DateTime? StartDate = null,
		DateTime? EndDate = null,
		string ChartGroupBy = "day" // day, week, month
	) : IRequest<AdminDashboardDto>;
}

