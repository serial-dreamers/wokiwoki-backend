using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.DTOs.Response
{
	public class WorkshopScheduleDto
	{
		public Guid Id { get; set; }
		public Guid WorkshopId { get; set; }
		public RecurrenceType RecurrenceType { get; set; }

		public string? DaysOfWeek { get; set; }
		public string? DaysOfMonth { get; set; }

		public TimeOnly StartTime { get; set; }
		public TimeOnly EndTime { get; set; }

		public DateTime ValidFrom { get; set; }
		public DateTime? ValidUntil { get; set; }

		public int? Capacity { get; set; }

	}
}
