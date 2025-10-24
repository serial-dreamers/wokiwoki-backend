using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;
using Wokiwoki.Domain.Events;

namespace Wokiwoki.Application.Features.Workshops.Commands.CreateWorkshop
{
	public record CreateWorkshopDraftCommand(
		string Title, 
		Guid OrganizationId,
		Guid CategoryId,
		List<Guid> TagIds
	) : IRequest<Guid>;
	public class CreateWorkshopDraftCommandHandler : IRequestHandler<CreateWorkshopDraftCommand, Guid>
	{
		private readonly IWorkshopRepository _workshopRepository;
		private readonly ITagRepository _tagRepository;
		private readonly IUuidService _uuidService;

		public CreateWorkshopDraftCommandHandler(IWorkshopRepository workshopRepository
			,ITagRepository tagRepository,
			IUuidService uuidService)
		{
			_workshopRepository = workshopRepository;
			_tagRepository = tagRepository;
			_uuidService = uuidService;
		}
		public async Task<Guid> Handle(CreateWorkshopDraftCommand request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(request.Title))
				throw new ArgumentException("Title is required");

			var id = _uuidService.NewGuid();
			var workshop = new Workshop
			{
				Id = id,
				Title = request.Title,
				Summary = string.Empty,
				Description = string.Empty,
				OrganizationId = request.OrganizationId,
				CategoryId = request.CategoryId,
				Status = WorkshopStatus.Draft,
				DefaultCapacity = 0,
				LikeCount = 0, 
				TotalBookings = 0 ,
				ReviewCount = 0,
				AverageRating = 0.0,
				UsesDefaultPricing = true,
				RefundPolicy = RefundPolicyType.NoRefund,
				DeliveryType = WorkshopDeliveryType.Offline,
				ScheduleType = WorkshopScheduleType.OneTime,
				IsActive = true,
				OnlineEventUrl = string.Empty,
				CreatedBy = "00000000-0000-0000-0000-000000000001",
				Created = DateTime.UtcNow,
				LastModified = DateTime.UtcNow,
				LastModifiedBy = "00000000-0000-0000-0000-000000000001",
				ImageUrl = string.Empty,
				DisplayAddress = string.Empty,  
			};
			if (request.TagIds != null && request.TagIds.Any())
			{
				var tags = await _tagRepository.GetTagsByCategory(request.CategoryId, cancellationToken);
				var validTags = tags.Where(t => request.TagIds.Contains(t.Id)).ToList();
				foreach (var item in validTags)
				{
					workshop.Tags.Add(item);
				}
			}
			var createdWorkshop = await _workshopRepository.CreateAsync(workshop);
			if (createdWorkshop == null)
				throw new Exception("Create workshop failed");

			workshop.AddDomainEvent(new WorkshopCreatedEvent(createdWorkshop));

			return createdWorkshop.Id;
		}
	}
}
