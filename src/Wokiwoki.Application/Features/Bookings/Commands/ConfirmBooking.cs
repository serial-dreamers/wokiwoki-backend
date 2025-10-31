using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Application.Features.Bookings.Commands
{
    public sealed record ConfirmBookingCommand(
        Guid Id,
        string Authorization

        ) : IRequest<bool>;
    public class ConfirmBooking : IRequestHandler<ConfirmBookingCommand, bool>
    {
        private readonly IBookingRepository _repository;
        public ConfirmBooking( IBookingRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
        {
            var sepayApiKey = Environment.GetEnvironmentVariable("SepayApiKey");

            // Header SePay gửi: "Authorization": "Apikey <API_KEY>"
            if (!request.Authorization.StartsWith("Apikey ", StringComparison.OrdinalIgnoreCase))
                return false;

            var apiKey = request.Authorization.Replace("Apikey ", "", StringComparison.OrdinalIgnoreCase).Trim();

            if (apiKey != sepayApiKey)
                return false;
            return await _repository.ConfirmBooking(request.Id, cancellationToken);
        }
    }
}
