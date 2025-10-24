using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.Workshops.Commands.UpdateWorkshop
{
    public record UpdateWorkshopCommand(
        Guid Id,
        string Title,
        Guid OrganizationId,
        Guid CategoryId,
        List<Guid> TagIds,
        WorkshopDeliveryType DeliveryType,
        WorkshopScheduleType ScheduleType,
        string Summary,
        int? DurationMinutes,
        int DefaultCapacity,
        string Description,
        string ImageUrl,
        string? OnlineEventUrl,
        RefundPolicyType RefundPolicy,
        int? RegistrationDeadlineHours


    ) : IRequest<Guid>;
    public class UpdateWorkshop
    {
    }
}
