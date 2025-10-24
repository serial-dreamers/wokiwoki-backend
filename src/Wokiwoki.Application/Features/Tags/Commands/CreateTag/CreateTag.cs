using AutoMapper;
using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Utils;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.Tags.Commands.CreateTag
{
	public record CreateTagCommand(
		string Name,
		string? Description,
		string? IconUrl
	) : IRequest<Guid>;

	public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Guid>
	{
		private readonly ITagRepository _tagRepository;
		private readonly IMapper _mapper;
		private readonly IUuidService _uuidService;
		private readonly IUserContext _userContext;


		public CreateTagCommandHandler(ITagRepository tagRepository, IMapper mapper, IUuidService uuidService, IUserContext userContext)
		{
			_tagRepository = tagRepository;
			_mapper = mapper;
			_uuidService = uuidService;
			_userContext = userContext;
		}

		public async Task<Guid> Handle(CreateTagCommand request, CancellationToken cancellationToken)
		{
			var tag = _mapper.Map<Tag>(request);
			tag.Id = _uuidService.NewGuid();
			tag.Created = TimeHelper.NowInVietnam();
			tag.IsActive = true;
			tag.CreatedBy = _userContext.UserId; 
			await _tagRepository.CreateAsync(tag, cancellationToken);

			return tag.Id;
		}
	}
}
