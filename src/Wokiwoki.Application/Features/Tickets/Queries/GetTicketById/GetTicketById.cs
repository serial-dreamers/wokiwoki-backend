using AutoMapper;
using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Tickets.Queries.GetTicketById
{
	public record GetTicketByIdQuery(
		Guid Id) : IRequest<TicketDto>;

	public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDto>
	{
		private readonly ITicketRepository _ticketRepository;
		private readonly IMapper _mapper;

		public GetTicketByIdQueryHandler(ITicketRepository ticketRepository, IMapper mapper)
		{
			_ticketRepository = ticketRepository;
			_mapper = mapper;
		}

		public async Task<TicketDto> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
		{
			var ticket = await _ticketRepository.GetTicketWithDetailsAsync(request.Id, cancellationToken);
			
			if (ticket == null)
			{
				throw new KeyNotFoundException($"Ticket with ID {request.Id} not found");
			}

			return _mapper.Map<TicketDto>(ticket);
		}
	}
}
