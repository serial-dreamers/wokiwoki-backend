namespace Wokiwoki.Api.Request
{
	public class GetOrganizationsByCategoryRequest
	{
		public List<Guid> CategoryIds { get; set; } = new();
		public int? LimitPerCategory { get; set; } = 3;
	}
}

