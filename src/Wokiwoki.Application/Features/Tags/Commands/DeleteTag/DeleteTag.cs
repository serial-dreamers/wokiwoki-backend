using MediatR;

namespace Wokiwoki.Application.Features.Tags.Commands.DeleteTag
{
	public record DeleteTagCommand(Guid Id) : IRequest<bool>;

	public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, bool>
	{
		private readonly ITagRepository _tagRepository;

		public DeleteTagCommandHandler(ITagRepository tagRepository)
		{
			_tagRepository = tagRepository;
		}

		public async Task<bool> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
		{
			var tag = await _tagRepository.GetByIdAsync(request.Id);
			if (tag == null)
				return false;

			return await _tagRepository.DeleteAsync(tag.Id, cancellationToken);
		}
	}
}
