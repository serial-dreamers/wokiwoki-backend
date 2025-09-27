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
using Wokiwoki.Domain.Events;

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
		private readonly ITagRepository _tagRepository;
		private readonly IMapper _mapper;
		private readonly IUuidService _uuidService;

		public CreateWorkshopCommandHandler(IWorkshopRepository workshopRepository, ITagRepository tagRepository, IMapper mapper, IUuidService uuidService)
		{
			_workshopRepository = workshopRepository;
			_tagRepository = tagRepository;
			_mapper = mapper;
			_uuidService = uuidService;
		}
		public async Task<Guid> Handle(CreateWorkshopCommand request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(request.Title))
				throw new ArgumentException("Name is required");

			var id = _uuidService.NewGuid();
			var workshop = _mapper.Map<Workshop>(request);
			workshop.Id = id;

			if(request.TagIds != null && request.TagIds.Any())
			{
				var tags = await _tagRepository.GetTagsByIdsAsync(request.TagIds);
				foreach(var tag in tags)
				{
					workshop.Tags.Add(tag);
				}
			}

			var createdWorkshop = await _workshopRepository.CreateAsync(workshop);

			if (createdWorkshop == null)
				throw new Exception("Create workshop failed");

			workshop.AddDomainEvent(new WorkshopCreatedEvent(createdWorkshop));

			return createdWorkshop.Id;
		}
	}
}
