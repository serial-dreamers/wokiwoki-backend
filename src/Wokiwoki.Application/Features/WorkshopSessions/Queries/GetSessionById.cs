using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Features.WorkshopSchedules.Commands.CreateSchedule;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.WorkshopSessions.Queries
{
    public sealed record GetSessionByIdQuery(Guid id) : IRequest<WorkshopSession>;
    public class GetSessionById : IRequestHandler<GetSessionByIdQuery, WorkshopSession>
    {
        private readonly IWorkshopSessionRepository _repo;
        private readonly IWorkshopRepository _workshopRepository;


        public GetSessionById(IWorkshopSessionRepository repo, IWorkshopRepository workshopRepository)
        {
            _repo = repo;
            _workshopRepository = workshopRepository;
        }
        public async Task<WorkshopSession> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
        {
            // 1️⃣ Lấy workshop hiện có (draft)
            var session = await _repo.GetByIdAsync(request.id);
            if (session == null)
                throw new Exception("Sesion not found");
            return session;
        }
    }
}
