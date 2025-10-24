namespace Wokiwoki.Application.Common.Utils
{
	public static class TimeHelper
	{
		private static readonly TimeZoneInfo VietnamTimeZone =
			TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

		public static DateTime NowInVietnam()
		{
			return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
		}

		public static DateTime ToVietnamTime(DateTime utcDateTime)
		{
			return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, VietnamTimeZone);
		}

		public static DateTime ToUtcFromVietnam(DateTime vietnamTime)
		{
			return TimeZoneInfo.ConvertTimeToUtc(vietnamTime, VietnamTimeZone);
		}
	}
}
