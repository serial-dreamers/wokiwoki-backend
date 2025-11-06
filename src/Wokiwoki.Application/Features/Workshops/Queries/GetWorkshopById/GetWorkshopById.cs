using AutoMapper;
using MediatR; 
using Wokiwoki.Application.DTOs.Response; 

namespace Wokiwoki.Application.Features.Workshops.Queries.GetWorkshop
{
	public sealed record GetWorkshopByIdQuery(Guid Id) : IRequest<WorkshopDto?>;

	public class GetWorkshopByIdQueryHandler : IRequestHandler<GetWorkshopByIdQuery, WorkshopDto?>
	{
		private readonly IWorkshopRepository _workshopRepository;
		private readonly IMapper _mapper;

		public GetWorkshopByIdQueryHandler(IWorkshopRepository workshopRepository, IMapper mapper)
		{
			_mapper = mapper;
			_workshopRepository = workshopRepository;
		}
		// Implement the handler logic here
		public async Task<WorkshopDto?> Handle(GetWorkshopByIdQuery request, CancellationToken cancellationToken)
		{
			var workshop = await _workshopRepository.GetWorkshopByIdAsync(request.Id, cancellationToken);

			if (workshop is null)
				return null;

			return _mapper.Map<WorkshopDto>(workshop);
		}
	}
}
 
