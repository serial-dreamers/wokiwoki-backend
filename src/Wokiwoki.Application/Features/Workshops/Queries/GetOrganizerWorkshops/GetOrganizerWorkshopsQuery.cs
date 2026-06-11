using MediatR;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Workshops.Queries.GetOrganizerWorkshops
{
	/// <summary>
	/// Query to get simple list of workshops for organizer (for dropdown/filter)
	/// </summary>
	public record GetOrganizerWorkshopsQuery : IRequest<List<Wokiwoki.Application.DTOs.Response.WorkshopSimpleDto>>;
}

