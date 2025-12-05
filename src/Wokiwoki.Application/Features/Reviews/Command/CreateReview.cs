using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Features.Reviews.Command
{
    public sealed record CreateReviewCommand(
        Guid WorkshopId,
        Guid BookingId,
        string FileName,
        string ContentType,
        long FileLength,
        Stream FileStream,
        string UserId,
        string Comment,
        int Rating
        ) : IRequest<Review>;
    public class CreateReview : IRequestHandler<CreateReviewCommand, Review>
    {
        private readonly IWorkshopRepository _workshopRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IReviewRepository _repo;
        private readonly IMapper _mapper;
        private readonly IBlobStorageService _blobStorageService;
        public CreateReview(IWorkshopRepository workshopRepo, IBookingRepository bookingRepo, IReviewRepository repo, IMapper mapper, IBlobStorageService blobStorageService)
        {
            _workshopRepo = workshopRepo;
            _bookingRepo = bookingRepo;
            _repo = repo;
            _mapper = mapper;
            _blobStorageService = blobStorageService;
        }

        public async Task<Review> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var w = await _workshopRepo.GetByIdAsync(request.WorkshopId);
            if (w == null)
            {
                return null;
            }
            var b = await _bookingRepo.GetByIdAsync(request.BookingId);
            if (b == null)
            {
                return null;
            }
            string imgUrl = await _blobStorageService.UploadFileAsync(
            request.FileStream,
            request.FileName,
            request.ContentType,
            request.FileLength,
            BlobContainerType.WorkshopMedia
            );
            var r = new Review
            {
                BookingId = request.BookingId,
                Comment = request.Comment,
                Created = DateTime.UtcNow,
                CreatedBy = request.UserId,
                Rating = request.Rating,
                ImageUrl = imgUrl,
            };
            var result = await _repo.CreateAsync(r);
            return result;
        }
    }
}
