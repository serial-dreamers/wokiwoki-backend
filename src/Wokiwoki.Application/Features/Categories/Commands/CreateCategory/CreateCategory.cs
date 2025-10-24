using AutoMapper;
using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Utils;
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
		private readonly IUserContext _userContext;

		public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper, IUuidService uuidService, IUserContext userContext)
		{
			_categoryRepository = categoryRepository;
			_mapper = mapper;
			_uuidService = uuidService;
			_userContext = userContext;
		}

		public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = _mapper.Map<Category>(request);
			category.Id = _uuidService.NewGuid();
			category.Created = TimeHelper.NowInVietnam();
			category.IsActive = true;
			category.CreatedBy = _userContext.UserId;

			await _categoryRepository.CreateAsync(category, cancellationToken);

			return category.Id;
		}
	}
}
