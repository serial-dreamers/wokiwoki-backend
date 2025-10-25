using AutoMapper;
using MediatR;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.WorkshopHeroMedias.Queries.GetHeroMediaById
{
	public record GetHeroMediaByIdQuery(Guid Id) : IRequest<WorkshopHeroMediaDto?>;
	public class GetHeroMediaByIdQueryHandler : IRequestHandler<GetHeroMediaByIdQuery, WorkshopHeroMediaDto?>
	{
		private readonly IWorkshopHeroMediaRepository _workshopHeroMediaRepository;
		private readonly IMapper _mapper;
		public GetHeroMediaByIdQueryHandler(IWorkshopHeroMediaRepository workshopHeroMediaRepository, IMapper mapper)
		{
			_workshopHeroMediaRepository = workshopHeroMediaRepository;
			_mapper = mapper;
		}
		public async Task<WorkshopHeroMediaDto?> Handle(GetHeroMediaByIdQuery request, CancellationToken cancellationToken)
		{
			var heroMedia = await _workshopHeroMediaRepository.GetByIdAsync(request.Id, cancellationToken);
			return _mapper.Map<WorkshopHeroMediaDto?>(heroMedia);
		}
	}
}
