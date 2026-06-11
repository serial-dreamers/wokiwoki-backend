using MediatR;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Dashboard.Queries.GetOrganizerDashboard
{
	/// <summary>
	/// Query to get dashboard statistics for organizer
	/// </summary>
	public record GetOrganizerDashboardQuery(
		DateTime? StartDate = null,
		DateTime? EndDate = null,
		Guid? WorkshopId = null,
		string ChartGroupBy = "day" // day, week, month
	) : IRequest<OrganizerDashboardDto>;
}

