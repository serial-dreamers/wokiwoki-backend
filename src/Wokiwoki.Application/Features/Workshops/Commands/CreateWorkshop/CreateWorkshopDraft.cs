using MediatR;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;
using Wokiwoki.Domain.Events;

namespace Wokiwoki.Application.Features.Workshops.Commands.CreateWorkshop
{
    public record CreateWorkshopDraftCommand(
        string? Id,
        string UserId,
        string Title,
        string Summary,
        Guid OrganizationId,
        Guid CategoryId,
        List<Guid> TagIds,
        decimal? StartingPrice,
        WorkshopDeliveryType DeliveryType,
        WorkshopScheduleType ScheduleType,
        int? DurationMinutes,
        int DefaultCapacity
    ) : IRequest<Guid>;

    public class CreateWorkshopDraftCommandHandler : IRequestHandler<CreateWorkshopDraftCommand, Guid>
    {
        private readonly IWorkshopRepository _workshopRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IUuidService _uuidService;

        public CreateWorkshopDraftCommandHandler(IWorkshopRepository workshopRepository
            , ITagRepository tagRepository,
            IUuidService uuidService)
        {
            _workshopRepository = workshopRepository;
            _tagRepository = tagRepository;
            _uuidService = uuidService;
        }
        public async Task<Guid> Handle(CreateWorkshopDraftCommand request, CancellationToken cancellationToken)
        {
            var result = new Workshop();
            if (string.IsNullOrEmpty(request.Title))
                throw new ArgumentException("Title is required");
            if (string.IsNullOrEmpty(request.Id))
            {
                var id = _uuidService.NewGuid();
                var workshop = new Workshop
                {
                    Id = id,
                    Title = request.Title,
                    Summary = request.Summary,
                    Description = string.Empty,
                    OrganizationId = request.OrganizationId,
                    CategoryId = request.CategoryId,
                    Status = WorkshopStatus.Draft,
                    DefaultCapacity = request.DefaultCapacity,
                    LikeCount = 0,
                    TotalBookings = 0,
                    ReviewCount = 0,
                    AverageRating = 0.0,
                    RefundPolicy = RefundPolicyType.NoRefund,
                    DeliveryType = WorkshopDeliveryType.Offline,
                    ScheduleType = WorkshopScheduleType.OneTime,
                    IsActive = true,
                    OnlineEventUrl = string.Empty,
                    Created = DateTime.UtcNow,
                    ImageUrl = string.Empty,
                    DisplayAddress = string.Empty,
                    CreatedBy = request.UserId
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
                result = await _workshopRepository.CreateAsync(workshop);

                if (result == null)
                    throw new Exception("Create workshop failed");

                workshop.AddDomainEvent(new WorkshopCreatedEvent(result));
            }
            else
            {
                // 🟠 UPDATE EXISTING WORKSHOP
                var existingWorkshop = await _workshopRepository.GetByIdAsync(Guid.Parse(request.Id));
                if (existingWorkshop == null)
                    throw new Exception("Workshop not found");

                existingWorkshop.Title = request.Title;
                existingWorkshop.Summary = request.Summary;
                existingWorkshop.CategoryId = request.CategoryId;
                existingWorkshop.OrganizationId = request.OrganizationId;
                existingWorkshop.DeliveryType = request.DeliveryType;
                existingWorkshop.ScheduleType = request.ScheduleType;
                existingWorkshop.DefaultCapacity = request.DefaultCapacity;
                existingWorkshop.StartingPrice = request.StartingPrice;
                existingWorkshop.LastModified = DateTime.UtcNow;
                existingWorkshop.LastModifiedBy = request.UserId;

                // 🏷️ Update tags
                if (request.TagIds != null)
                {
                    var tags = await _tagRepository.GetTagsByCategory(request.CategoryId, cancellationToken);
                    var validTags = tags.Where(t => request.TagIds.Contains(t.Id)).ToList();

                    // Clear và add lại để sync
                    existingWorkshop.Tags.Clear();
                    foreach (var tag in validTags)
                    {
                        existingWorkshop.Tags.Add(tag);
                    }
                }

                await _workshopRepository.UpdateTAsync(existingWorkshop.Id, existingWorkshop, cancellationToken);
                result = existingWorkshop;
            }
            return result.Id;
        }
    }
}
