using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.Workshops.Commands.UpdateWorkshop
{
    public record UpdateWorkshopDetailsCommand(
        Guid Id, 
		string imgUrl,
        string Description,
		string? DisplayAddress,
		double? Latitude,
	    double? Longitude,
		string? OnlineEventUrl,
		int? DurationMinutes,
		int? RegistrationDeadlineHours,
		RefundPolicyType RefundPolicy,
		string? RefundPolicyDescription,
		List<Guid>? HeroMediaIds 
	) : IRequest<Guid>;

    public class UpdateWorkshopDetails
    {
    }
}
