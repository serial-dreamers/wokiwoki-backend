using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.Workshops.Commands.CreateWorkshop
{
	public record CreateWorkshopCommand(
		string Title,
		string? ShortDescription,
		string Description,
		string ImageUrl,
		int Capacity,
		Guid OrganizationId,
		Guid CategoryId,
		List<Guid> TagIds
	) : IRequest<Guid>;


	public class CreateWorkshopCommandHandler : IRequestHandler<CreateWorkshopCommand, Guid>
	{
		private readonly IWorkshopRepository _workshopRepository;
		private readonly IMapper _mapper;
		private readonly IGuidGenerator _guidGenerator;

		public CreateWorkshopCommandHandler(IWorkshopRepository workshopRepository, IMapper mapper, IGuidGenerator guidGenerator)
		{
			_workshopRepository = workshopRepository;
			_mapper = mapper;
			_guidGenerator = guidGenerator;
		}
		public async Task<Guid> Handle(CreateWorkshopCommand request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(request.Title))
				throw new ArgumentException("Name is required");

			var id = _guidGenerator.NewGuid();
			var workshop = _mapper.Map<Workshop>(request);
			workshop.Id = id;

			var createdWorkshop = await _workshopRepository.CreateAsync(workshop);

			if (createdWorkshop == null)
				throw new Exception("Create workshop failed");

			return createdWorkshop.Id;
		}
	}
}
