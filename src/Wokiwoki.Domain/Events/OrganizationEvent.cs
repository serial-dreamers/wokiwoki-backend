
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Domain.Events
{
	public class OrganizationCreateEvent
	{
		public Organization Organization { get; }

		public OrganizationCreateEvent(Organization organization)
		{
			Organization = organization;
		}
	}
}
