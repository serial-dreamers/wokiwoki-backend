using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Workshops.Queries.GetWorkshopSessionsByDateRange
{
	public record GetWorkshopSessionsByDateRangeQuery : IRequest<List<WorkshopSessionCalendarDto>>
	{
		public Guid OrganizationId { get; init; }
		public DateTime StartDate { get; init; }
		public DateTime EndDate { get; init; }
	}

	public class GetWorkshopSessionsByDateRangeHandler : IRequestHandler<GetWorkshopSessionsByDateRangeQuery, List<WorkshopSessionCalendarDto>>
	{
		private readonly IWorkshopRepository _workshopRepository;

		public GetWorkshopSessionsByDateRangeHandler(IWorkshopRepository workshopRepository)
		{
			_workshopRepository = workshopRepository;
		}

		public async Task<List<WorkshopSessionCalendarDto>> Handle(GetWorkshopSessionsByDateRangeQuery request, CancellationToken cancellationToken)
		{
			var sessions = await _workshopRepository.GetSessionsByOrganizationAndDateRangeAsync(
				request.OrganizationId,
				request.StartDate,
				request.EndDate,
				cancellationToken
			);

			return sessions.Select(s => new WorkshopSessionCalendarDto
			{
				Id = s.Id,
				Title = s.Title,
				Description = s.Description,
				StartTime = s.StartTime,
				EndTime = s.EndTime,
				Capacity = s.Capacity,
				BookedCount = s.BookedCount,
				WorkshopId = s.WorkshopId,
				WorkshopTitle = s.Workshop.Title,
				WorkshopImageUrl = s.Workshop.ImageUrl,
				WorkshopStatus = s.Workshop.Status,
				DeliveryType = s.Workshop.DeliveryType,
				DisplayAddress = s.Street != null 
					? $"{s.Street}, {s.Commune}, {s.Province}" 
					: s.Workshop.DisplayAddress,
				OnlineEventUrl = s.Workshop.OnlineEventUrl,
				StartingPrice = s.Workshop.StartingPrice
			}).ToList();
		}
	}
}

