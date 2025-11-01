using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Utils;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Infrastructure.Repositories
{
    public class WorkshopSessionRepository : BaseRepo<WorkshopSession, Guid>, IWorkshopSessionRepository
    {
        private readonly IUuidService _uuidService;
        public WorkshopSessionRepository(WokiwokiDbContext context, IUuidService uuidService) : base(context)
        { 
            _uuidService = uuidService;
        }

        public async Task<List<WorkshopSession>> GetSessionByScheduleId(Guid scheduleId, CancellationToken cancellationToken)
        {
            var schedule = await _context.WorkshopSchedules.FirstOrDefaultAsync(s => s.Id == scheduleId);
            if(schedule == null)
            {
                return null;
            }
            var result = await _context.WorkshopSessions.Where(s => s.ScheduleId == scheduleId).ToListAsync();
            return result;
        }
        public async Task<List<WorkshopSession>> Create1MonthSession(Guid scheduleId, WorkshopSession session, CancellationToken cancellationToken = default)
        {
            var schedule = await _context.WorkshopSchedules
                .Include(s => s.Workshop)
                .FirstOrDefaultAsync(s => s.Id == scheduleId, cancellationToken);

            if (schedule == null)
                throw new Exception("Schedule not found");
             
            var latestSession = await _context.WorkshopSessions
                .Where(x => x.ScheduleId == scheduleId)
                .OrderByDescending(x => x.StartTime)
                .FirstOrDefaultAsync(cancellationToken);

            var now = TimeHelper.NowInVietnam();
            DateTime startDate;

            if (latestSession == null)
            {
                startDate = now;
            }
            else
            {
                startDate = latestSession.StartTime > now
                    ? latestSession.StartTime
                    : now;
            }

            var endDate = startDate.AddMonths(1);

            var createdSessions = new List<WorkshopSession>();
            var dates = new List<DateTime>();

            // Weekly recurrence
            if (schedule.RecurrenceType == RecurrenceType.Weekly && !string.IsNullOrEmpty(schedule.DaysOfWeek))
            {
                var days = schedule.DaysOfWeek.Split(',')
                    .Select(d => (DayOfWeek)int.Parse(d.Trim()))
                    .ToList();

                for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    if (days.Contains(date.DayOfWeek))
                        dates.Add(date);
                }
            }
            // Monthly recurrence
            else if (schedule.RecurrenceType == RecurrenceType.Monthly && !string.IsNullOrEmpty(schedule.DaysOfMonth))
            {
                var daysOfMonth = schedule.DaysOfMonth.Split(',')
                    .Select(int.Parse)
                    .Where(d => d >= 1 && d <= 31)
                    .ToList();

                for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    if (daysOfMonth.Contains(date.Day))
                        dates.Add(date);
                }
            }
            else if (schedule.RecurrenceType == RecurrenceType.Daily)
            {
                for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    dates.Add(date);
                }
            }

            foreach (var date in dates)
            { 
				var entity = new WorkshopSession
                {
                    Id = _uuidService.NewGuid(),
                    WorkshopId = schedule.WorkshopId,
                    ScheduleId = schedule.Id,
                    Title = session.Title,
                    Description = session.Description,
                    Street = session.Street,
                    Commune = session.Commune,
                    Province = session.Province,
                    Latitude = session.Latitude,
                    Longitude = session.Longitude,
                    AgeRestrictionType = session.AgeRestrictionType,
                    MinimumAge = session.MinimumAge,
                    ParkingType = session.ParkingType,
                    ParkingDescription = session.ParkingDescription,
                    Capacity = schedule.Capacity ?? session.Capacity,
                    BookedCount = 0,
                    StartTime = date.Add(schedule.StartTime.ToTimeSpan()),
                    EndTime = date.Add(schedule.EndTime.ToTimeSpan()),
                    IsActive = true
                };

                var created = await CreateAsync(entity, cancellationToken);
                createdSessions.Add(created);
            }

            return createdSessions;
        }

		public async Task<List<WorkshopSession>> GetSessionsWeekByScheduleId(
	            Guid scheduleId,
	            DateTime? startDate,
	            DateTime? endDate,
	            CancellationToken cancellationToken)
		{
			var query = _context.WorkshopSessions
				.Where(x => x.ScheduleId == scheduleId);

			if (startDate.HasValue)
				query = query.Where(x => x.StartTime >= startDate.Value);

			if (endDate.HasValue)
				query = query.Where(x => x.EndTime <= endDate.Value);

			query = query.OrderBy(x => x.StartTime);

			return await query.ToListAsync(cancellationToken);
		}

	}
}

