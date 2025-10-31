using AutoMapper;
using MediatR;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Domain.Entities; 

namespace Wokiwoki.Application.Features.WorkshopHeroMedias.Queries.GetHeroMedias
{
	public record WorkshopHeroMediaDtoWithWsId
	{
		public Guid Id { get; set; }
		public Guid MediaId { get; init; }
		public Guid WorkshopId { get; init; }
		public bool IsActive { get; init; }
		public int HeroType { get; init; }  
		public string ImageUrl { get; init; }  
	}

	public record GetHeroMediasByWorkshopIdQuery(
		Guid workshopId) : IRequest<List<WorkshopHeroMediaDtoWithWsId>>;

	public class GetHeroMediasByWorkshopIdQueryHandler : IRequestHandler<GetHeroMediasByWorkshopIdQuery, List<WorkshopHeroMediaDtoWithWsId>>
	{
		private readonly IWorkshopHeroMediaRepository _workshopHeroMediaRepository;
		private readonly IMapper _mapper;
		public GetHeroMediasByWorkshopIdQueryHandler(IWorkshopHeroMediaRepository workshopHeroMediaRepository,
			IMapper mapper)
		{
			_workshopHeroMediaRepository = workshopHeroMediaRepository;
			_mapper = mapper;
		}
		public async Task<List<WorkshopHeroMediaDtoWithWsId>> Handle(GetHeroMediasByWorkshopIdQuery request, CancellationToken cancellationToken)
		{
			var entities = await _workshopHeroMediaRepository.GetHeroMediasByWorkshopId(request.workshopId);
			return _mapper.Map<List<WorkshopHeroMediaDtoWithWsId>>(entities);
		}
	}
}
