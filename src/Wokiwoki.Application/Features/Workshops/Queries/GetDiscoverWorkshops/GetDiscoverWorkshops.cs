using AutoMapper;
using MediatR;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.Workshops.Queries.GetDiscoverWorkshops
{
	public enum DiscoverFilterType
	{
		All = 0,
		My = 1,
		Today = 2,
		Week = 3
	}

	public sealed record GetDiscoverWorkshopsQuery(int Limit = 8, DiscoverFilterType FilterType = DiscoverFilterType.All, string? UserId = null) : IRequest<List<DiscoverWorkshopDto>>;

	public class GetDiscoverWorkshopsQueryHandler : IRequestHandler<GetDiscoverWorkshopsQuery, List<DiscoverWorkshopDto>>
	{
		private readonly IWorkshopRepository _workshopRepository;
		private readonly IOrganizationRepository _organizationRepository;
		private readonly IUserTagPreferenceRepository _userTagPreferenceRepository;
		private readonly IMapper _mapper;

		public GetDiscoverWorkshopsQueryHandler(
			IWorkshopRepository workshopRepository,
			IOrganizationRepository organizationRepository,
			IUserTagPreferenceRepository userTagPreferenceRepository,
			IMapper mapper)
		{
			_workshopRepository = workshopRepository;
			_organizationRepository = organizationRepository;
			_userTagPreferenceRepository = userTagPreferenceRepository;
			_mapper = mapper;
		}

		public async Task<List<DiscoverWorkshopDto>> Handle(GetDiscoverWorkshopsQuery request, CancellationToken cancellationToken)
		{
			List<Workshop> workshops;

			switch (request.FilterType)
			{
				case DiscoverFilterType.My:
					workshops = await GetPersonalizedWorkshopsAsync(request.UserId, request.Limit, cancellationToken);
					break;
				case DiscoverFilterType.Today:
					workshops = await GetTodayWorkshopsAsync(request.Limit, cancellationToken);
					break;
				case DiscoverFilterType.Week:
					workshops = await GetWeekWorkshopsAsync(request.Limit, cancellationToken);
					break;
				default: // All
					workshops = await _workshopRepository.GetDiscoverWorkshopsAsync(request.Limit, cancellationToken);
					break;
			}

			var result = new List<DiscoverWorkshopDto>();

			foreach (var workshop in workshops)
			{
				var discoverWorkshop = _mapper.Map<DiscoverWorkshopDto>(workshop);

				// Get organization info
				if (workshop.OrganizationId != Guid.Empty)
				{
					var organization = await _organizationRepository.GetByIdAsync(workshop.OrganizationId);
					if (organization != null)
					{
						discoverWorkshop.Organization = _mapper.Map<DiscoverOrganizationDto>(organization);
					}
				}

				result.Add(discoverWorkshop);
			}

			return result;
		}

		private async Task<List<Workshop>> GetPersonalizedWorkshopsAsync(string? userId, int limit, CancellationToken cancellationToken)
		{
			var workshops = new List<Workshop>();

			// Get user preferences
			if (!string.IsNullOrEmpty(userId))
			{
				var userPreferences = await _userTagPreferenceRepository.GetByUserIdAsync(userId, cancellationToken);
				if (userPreferences.Any())
				{
					var preferredTagIds = userPreferences.Select(p => p.TagId).ToList();
					workshops = await _workshopRepository.GetWorkshopsByTagIdsAsync(preferredTagIds, limit, cancellationToken);
				}
			}

			// If not enough workshops based on preferences, fill with random popular workshops
			if (workshops.Count < limit)
			{
				var remainingLimit = limit - workshops.Count;
				var popularWorkshops = await _workshopRepository.GetDiscoverWorkshopsAsync(remainingLimit * 2, cancellationToken);

				// Filter out workshops already in the list
				var existingIds = workshops.Select(w => w.Id).ToHashSet();
				var additionalWorkshops = popularWorkshops
					.Where(w => !existingIds.Contains(w.Id))
					.Take(remainingLimit)
					.ToList();

				workshops.AddRange(additionalWorkshops);
			}

			return workshops.Take(limit).ToList();
		}

		private async Task<List<Workshop>> GetTodayWorkshopsAsync(int limit, CancellationToken cancellationToken)
		{
			var utcNow = DateTime.UtcNow;
			var startOfDayUtc = utcNow.Date;
			var endOfDayUtc = startOfDayUtc.AddDays(1).AddTicks(-1);

			return await _workshopRepository.GetWorkshopsByDateRangeAsync(startOfDayUtc, endOfDayUtc, limit, cancellationToken);
		}

		private async Task<List<Workshop>> GetWeekWorkshopsAsync(int limit, CancellationToken cancellationToken)
		{
			var utcNow = DateTime.UtcNow;
			var dayOfWeek = (int)utcNow.DayOfWeek;

			// Tính thứ Hai đầu tuần (UTC)
			var startOfWeekUtc = utcNow.AddDays(-(dayOfWeek - 1)).Date;
			if (dayOfWeek == 0) // Nếu hôm nay là Chủ nhật
				startOfWeekUtc = utcNow.AddDays(-6).Date;

			// Chủ nhật cuối tuần (UTC)
			var endOfWeekUtc = startOfWeekUtc.AddDays(7).AddTicks(-1);

			return await _workshopRepository.GetWorkshopsByDateRangeAsync(startOfWeekUtc, endOfWeekUtc, limit, cancellationToken);
		}
	}
}

