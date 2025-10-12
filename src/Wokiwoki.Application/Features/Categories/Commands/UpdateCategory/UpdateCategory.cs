using AutoMapper;
using MediatR;
using Wokiwoki.Application.Common.Interfaces.Repositories;

namespace Wokiwoki.Application.Features.Categories.Commands.UpdateCategory
{
	public record UpdateCategoryCommand(
		Guid Id,
		string Name,
		string? Description,
		string? IconUrl,
		string? ImageUrl
	) : IRequest<bool>;

	public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly IMapper _mapper;

		public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
		{
			_categoryRepository = categoryRepository;
			_mapper = mapper;
		}

		public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = await _categoryRepository.GetByIdAsync(request.Id);
			if (category == null)
				throw new KeyNotFoundException($"Category with Id '{request.Id}' not found.");

			_mapper.Map(request, category);
			category.LastModified = DateTime.UtcNow;

			return await _categoryRepository.UpdateAsync(category.Id, category); 
		}
	}
}
