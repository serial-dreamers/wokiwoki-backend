using AutoMapper;
using MediatR;
using Wokiwoki.Application.DTOs;

namespace Wokiwoki.Application.Features.Tags.Queries.GetTagById
{
	public record GetTagByIdQuery(Guid tagId) : IRequest<TagDto>;


	public class GetTagByIdCommandHandler : IRequestHandler<GetTagByIdQuery, TagDto>
	{
		private readonly ITagRepository _tagRepository;
		private readonly IMapper _mapper;


		public GetTagByIdCommandHandler(ITagRepository tagRepository, IMapper mapper)
		{
			_tagRepository = tagRepository;
			_mapper = mapper;
		}
		public async Task<TagDto> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
		{
			var tag = await _tagRepository.GetByIdAsync(request.tagId);
			return _mapper.Map<TagDto>(tag);
		}
	}
}
