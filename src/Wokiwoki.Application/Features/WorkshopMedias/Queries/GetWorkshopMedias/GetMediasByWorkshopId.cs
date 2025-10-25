
using AutoMapper;
using MediatR;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.WorkshopMedias.Queries.GetWorkshopMedias
{
	public record GetMediasByWorkshopIdQuery(Guid workshopId) : IRequest<List<WorkshopMediaDto>>;

	public class GetMediasByWorkshopIdQueryHandler : IRequestHandler<GetMediasByWorkshopIdQuery, List<WorkshopMediaDto>>
	{
		private readonly IWorkshopMediaRepository _workshopMediaRepository;
		private readonly IMapper _mapper;
		public GetMediasByWorkshopIdQueryHandler(IWorkshopMediaRepository workshopMediaRepository, IMapper mapper)
		{
			_workshopMediaRepository = workshopMediaRepository;
			_mapper = mapper;
		}

		public async Task<List<WorkshopMediaDto>> Handle(GetMediasByWorkshopIdQuery request, CancellationToken cancellationToken)
		{
			var medias = await  _workshopMediaRepository.GetActiveMediaByWorkshopIdAsync(request.workshopId);
			return _mapper.Map<List<WorkshopMediaDto>>(medias);
		}
	}
}
