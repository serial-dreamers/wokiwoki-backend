namespace Wokiwoki.Application.Common.Utils
{
	public static class TimeHelper
	{
		private static readonly TimeZoneInfo VietnamTimeZone =
			TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

		public static DateTime NowInVietnam()
		{
			var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
			//return DateTime.SpecifyKind(vietnamTime, DateTimeKind.Local);
			return DateTime.UtcNow;

		}

		public static DateTime ToVietnamTime(DateTime utcDateTime)
		{
			var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, VietnamTimeZone);
			//return DateTime.SpecifyKind(vietnamTime, DateTimeKind.Local);
			return DateTime.UtcNow;

		}

		public static DateTime ToUtcFromVietnam(DateTime vietnamTime)
		{
			if (vietnamTime.Kind == DateTimeKind.Unspecified)
				vietnamTime = DateTime.SpecifyKind(vietnamTime, DateTimeKind.Local);
			//return TimeZoneInfo.ConvertTimeToUtc(vietnamTime, VietnamTimeZone);
			return DateTime.UtcNow;

		}
	}

}
