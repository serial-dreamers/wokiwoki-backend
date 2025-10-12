using MediatR;

namespace Wokiwoki.Application.Features.Categories.Commands.DeleteCategory
{
	public record DeleteCategoryCommand(Guid Id) : IRequest<bool>;

	public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
	{
		private readonly ICategoryRepository _categoryRepository;

		public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
		{
			_categoryRepository = categoryRepository;
		}

		public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = await _categoryRepository.GetByIdAsync(request.Id);
			if (category == null)
				throw new KeyNotFoundException($"Category with Id '{request.Id}' not found.");

			
			return await _categoryRepository.Delete(category.Id);
		}
	}
}
