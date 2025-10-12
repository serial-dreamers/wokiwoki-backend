using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Application.Common.Models
{
	public class PaginatedList<T>
	{
		public int PageNumber { get; }
		public int TotalCount { get; } 
		public int TotalPages { get; }
		public IReadOnlyCollection<T> Records { get; }

		public PaginatedList(IReadOnlyCollection<T> records, int count, int pageNumber, int pageSize)
		{
			PageNumber = pageNumber;
			TotalCount = count;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
			Records = records;
		}

		public bool HasPreviousPage => PageNumber > 1;
		public bool HasNextPage => PageNumber < TotalPages;
	}
}
