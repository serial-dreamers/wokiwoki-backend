using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Wokiwoki.Application.Features.Sepay.QRCode
{
    public sealed record GenerateQRCodeCommand(
        decimal amount,
        string des
        ) : IRequest<string>;
    public class GenerateQRCode : IRequestHandler<GenerateQRCodeCommand, string>
    {
        private readonly IBookingRepository _bookingRepository;
        public GenerateQRCode ( IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        public async Task<string> Handle(GenerateQRCodeCommand request, CancellationToken cancellationToken)
        {
            string result = "Booking not found";
            var booking = await _bookingRepository.GetByIdAsync(Guid.Parse(request.des));
            if(booking != null && request.amount >= 2000)
            {
                result = $"https://qr.sepay.vn/img?acc=0868507859&bank=Mbbank&amount={request.amount}&des={request.des}";
            }
            return result;
        }
    }
}
