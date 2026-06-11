using System.Text.Json.Serialization; 

namespace Wokiwoki.Application.Common.Models
{
	public class PaginatedList<T>
	{ 
		public int PageNumber { get; }
		 
		public int TotalCount { get; }
		 
		public int TotalPages { get; }
		 
		public bool HasPreviousPage => PageNumber > 1;
		 
		public bool HasNextPage => PageNumber < TotalPages;
		 
		public IReadOnlyCollection<T> Records { get; }

	public PaginatedList(IReadOnlyCollection<T> records, int count, int pageNumber, int pageSize)
	{
		PageNumber = pageNumber;
		TotalCount = count;
		TotalPages = (int)Math.Ceiling(count / (double)pageSize);
		Records = records;
	}

	public static PaginatedList<T> Create(IReadOnlyCollection<T> records, int count, int pageNumber, int pageSize)
	{
		return new PaginatedList<T>(records, count, pageNumber, pageSize);
	}
}
}
