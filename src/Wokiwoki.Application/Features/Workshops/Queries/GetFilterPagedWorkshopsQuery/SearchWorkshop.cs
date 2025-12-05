using AutoMapper;
using MediatR;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Workshops.Queries.GetFilterPagedWorkshopsQuery
{
	public record SearchWorkshopQuery(
		string? Title,
		Guid? CategoryId,
		List<Guid>? TagIds,
		string? DateFilterType,
		DateTime? StartDate,
		DateTime? EndDate,
		bool? IsFree,
		double? Latitude,
		double? Longitude,
		double? RadiusInKm,  
		int PageNumber = 1,
		int PageSize = 10
		) : IRequest<PaginatedList<SearchWorkshopDto>>;

	public class SearchWorkshopQueryHandler : IRequestHandler<SearchWorkshopQuery, PaginatedList<SearchWorkshopDto>>
	{
		private readonly IWorkshopRepository _workshopRepository;
		private readonly IMapper _mapper;

		public SearchWorkshopQueryHandler(IWorkshopRepository workshopRepository, IMapper mapper)
		{
			_workshopRepository = workshopRepository;
			_mapper = mapper;
		}

		public async Task<PaginatedList<SearchWorkshopDto>> Handle(SearchWorkshopQuery request, CancellationToken cancellationToken)
		{
			var workshops = await _workshopRepository.SearchAsync(request, cancellationToken);

			// Map workshops to SearchWorkshopDto and include session information
			var searchResults = new List<SearchWorkshopDto>();

		foreach (var workshop in workshops.Records)
		{
			var searchDto = _mapper.Map<SearchWorkshopDto>(workshop);

			// Find the first upcoming session for this workshop
			var upcomingSession = workshop.WorkshopSessions?
				.Where(s => s.IsActive && s.StartTime > DateTime.UtcNow)
				.OrderBy(s => s.StartTime)
				.FirstOrDefault();

			if (upcomingSession != null)
			{
				searchDto.StartTime = upcomingSession.StartTime;
				searchDto.EndTime = upcomingSession.EndTime;
				searchDto.Session = _mapper.Map<WorkshopSessionDto>(upcomingSession);
			}

			// Set coordinates: priority to workshop level, fallback to session
			if (workshop.Latitude.HasValue && workshop.Longitude.HasValue &&
				workshop.Latitude.Value != 0 && workshop.Longitude.Value != 0)
			{
				// Use workshop level coordinates
				searchDto.Latitude = workshop.Latitude.Value;
				searchDto.Longitude = workshop.Longitude.Value;
				searchDto.DisplayAddress = workshop.DisplayAddress;
			}
			else if (upcomingSession != null && 
				upcomingSession.Latitude.HasValue && upcomingSession.Longitude.HasValue &&
				upcomingSession.Latitude.Value != 0 && upcomingSession.Longitude.Value != 0)
			{
				// Fallback to session coordinates
				searchDto.Latitude = upcomingSession.Latitude.Value;
				searchDto.Longitude = upcomingSession.Longitude.Value;
				searchDto.DisplayAddress = upcomingSession.Street+ ", " + upcomingSession.Commune + " " + upcomingSession.Province;
			}

			searchResults.Add(searchDto);
		}

			return new PaginatedList<SearchWorkshopDto>(
				searchResults,
			    workshops.TotalCount,
				workshops.PageNumber,
				request.PageSize
			);
		}
	}
}
