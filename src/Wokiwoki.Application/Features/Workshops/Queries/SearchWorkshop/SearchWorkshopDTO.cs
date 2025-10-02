using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Features.Workshops.Queries.SearchWorkshop
{
    public class SearchWorkshopDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? ShortDescription { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
        public WorkshopSession Session { get; set; }
    }
}
