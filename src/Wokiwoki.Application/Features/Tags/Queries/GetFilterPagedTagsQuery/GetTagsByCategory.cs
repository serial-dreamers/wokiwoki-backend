using AutoMapper;
using MediatR;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Tags.Queries.GetFilterPagedTagsQuery
{
	public record GetTagsByCategoryQuery(Guid CategoryId) : IRequest<List<TagDto>>;

	public class GetTagsByCategoryCommandHandler : IRequestHandler<GetTagsByCategoryQuery, List<TagDto>>
	{
		private readonly ITagRepository _tagRepository;
		private readonly IMapper _mapper;

		public GetTagsByCategoryCommandHandler(ITagRepository tagRepository, IMapper  mapper)
		{
			_tagRepository = tagRepository;
			_mapper = mapper;
		}

		public async Task<List<TagDto>> Handle(GetTagsByCategoryQuery request, CancellationToken cancellationToken)
		{
			var tags = await _tagRepository.GetTagsByCategory(request.CategoryId, cancellationToken); 

			return _mapper.Map<List<TagDto>>(tags);
		}
	}
}
