using AutoMapper;
using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Utils;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Tags.Commands.UpdateTag
{
	public record UpdateTagCommand(
		Guid Id,
		string Name,
		string? Description,
		string? IconUrl
	) : IRequest<TagDto>;

	public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, TagDto>
	{
		private readonly ITagRepository _tagRepository;
		private readonly IMapper _mapper;
		private readonly IUserContext _userContext; 

		public UpdateTagCommandHandler(ITagRepository tagRepository, IMapper mapper, IUserContext userContext)
		{
			_tagRepository = tagRepository;
			_mapper = mapper;
			_userContext = userContext;
		}

		public async Task<TagDto> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
		{
			var tag = await _tagRepository.GetByIdAsync(request.Id, cancellationToken);
			if (tag == null)
				throw new KeyNotFoundException($"Tag with Id '{request.Id}' not found.");

			_mapper.Map(request, tag);
			tag.LastModified = TimeHelper.NowInVietnam();
			tag.LastModifiedBy = _userContext.UserId;

			var updatedTag = await _tagRepository.UpdateAsync(tag.Id, tag, cancellationToken);
			return _mapper.Map<TagDto>(updatedTag);
		}
	}
}
