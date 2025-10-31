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
using Wokiwoki.Domain.Enums;
using Wokiwoki.Domain.Events;

namespace Wokiwoki.Application.Features.Workshops.Commands.CreateWorkshop
{
	public record CreateWorkshopCommand(
        Guid Id,
        string Description,
        string ImageUrl,
        string? OnlineEventUrl,
        RefundPolicyType RefundPolicy,
        string? RefundPolicyDescription,
        int? RegistrationDeadlineHours

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
            // 1️⃣ Lấy workshop hiện có (draft)
            var workshop = await _workshopRepository.GetByIdAsync(request.Id);
            if (workshop == null)
                throw new Exception("Workshop not found");

            // 2️⃣ Map những field được phép từ request sang entity
            _mapper.Map(request, workshop);

            // 3️⃣ Cập nhật lại DB
            var updatedWorkshop = await _workshopRepository.UpdateTAsync(request.Id, workshop, cancellationToken);

            if (updatedWorkshop == null)
                throw new Exception("Update workshop failed");

            // 4️⃣ Trả về ID
            return updatedWorkshop.Id;
        }
	}
}
