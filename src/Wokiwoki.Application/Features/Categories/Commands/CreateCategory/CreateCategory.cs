using AutoMapper;
using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.Categories.Commands.CreateCategory
{
	public record CreateCategoryCommand(
		string Name,
		string? Description,
		string? IconUrl,
		string? ImageUrl
	) : IRequest<Guid>;

	public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly IMapper _mapper;
		private readonly IUuidService _uuidService;

		public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper, IUuidService uuidService)
		{
			_categoryRepository = categoryRepository;
			_mapper = mapper;
			_uuidService = uuidService;
		}

		public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = _mapper.Map<Category>(request);
			category.Id = _uuidService.NewGuid();
			category.Created = DateTime.UtcNow;
			category.IsActive = true;

			await _categoryRepository.CreateAsync(category);
			return category.Id;
		}
	}
}
