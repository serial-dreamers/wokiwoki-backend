using AutoMapper;
using MediatR;
using Wokiwoki.Application.DTOs.Response;

namespace Wokiwoki.Application.Features.Categories.Queries.GetCategoryById
{
	public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto?>;

	public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly IMapper _mapper;

		public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
		{
			_categoryRepository = categoryRepository;
			_mapper = mapper;
		}

		public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
		{
			var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

			if (category is null)
				return null;

			return _mapper.Map<CategoryDto>(category);
		}
	}
}
