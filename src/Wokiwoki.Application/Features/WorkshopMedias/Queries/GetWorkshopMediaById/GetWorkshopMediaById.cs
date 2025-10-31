using AutoMapper;
using MediatR;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.WorkshopMedias.Queries.GetWorkshopMediaById
{
	public sealed record GetWorkshopMediaByIdQuery(Guid Id) : IRequest<WorkshopMediaDto?>;

	public class GetWorkshopMediaByIdQueryHandler : IRequestHandler<GetWorkshopMediaByIdQuery, WorkshopMediaDto?>
	{
		private readonly IWorkshopMediaRepository _workshopMediaRepository;
		private readonly IMapper _mapper;
		public GetWorkshopMediaByIdQueryHandler(IWorkshopMediaRepository workshopMediaRepository, IMapper mapper)
		{
			_workshopMediaRepository = workshopMediaRepository;
			_mapper = mapper;
		}
		public async Task<WorkshopMediaDto?> Handle(GetWorkshopMediaByIdQuery request, CancellationToken cancellationToken)
		{
			var workshopMedia = await _workshopMediaRepository.GetByIdAsync(request.Id, cancellationToken);
			return _mapper.Map<WorkshopMediaDto?>(workshopMedia);
		}
	}
}
