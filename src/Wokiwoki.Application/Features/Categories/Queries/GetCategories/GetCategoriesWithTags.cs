using AutoMapper;
using MediatR;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Categories.Queries.GetCategories
{
	public record GetCategoriesWithTagsQuery : IRequest<List<CategoryDto>>;

	public class GetCategoriesWithTagsQueryHandler : IRequestHandler<GetCategoriesWithTagsQuery, List<CategoryDto>>
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly IMapper _mapper;

		public GetCategoriesWithTagsQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
		{
			_categoryRepository = categoryRepository;
			_mapper = mapper;
		}
		public async Task<List<CategoryDto>> Handle(GetCategoriesWithTagsQuery request, CancellationToken cancellationToken)
		{
			var categories = await _categoryRepository.GetAllCategoriesWithTagsAsync(cancellationToken);
			return _mapper.Map<List<CategoryDto>>(categories);
		}
	} 
}
