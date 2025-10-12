using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Application.DTOs.Response
{
	public class ReviewDto
	{
		public Guid Id { get; set; }

		public Guid WorkshopId { get; set; }

		public string ImageUrl { get; set; } = null!;

		public string UserId { get; set; } = null!;

		public int Rating { get; set; }
	}
}
