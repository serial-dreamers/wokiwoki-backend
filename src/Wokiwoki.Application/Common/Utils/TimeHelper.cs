namespace Wokiwoki.Application.Common.Utils
{
	public static class TimeHelper
	{
		public static readonly TimeZoneInfo VietnamTimeZone =
			TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

		/// <summary>
		/// Lấy giờ hiện tại ở Việt Nam (UTC+7)
		/// </summary>
		public static DateTime NowInVietnam()
		{
			return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
		}

		/// <summary>
		/// Chuyển từ UTC sang giờ Việt Nam
		/// </summary>
		public static DateTime ToVietnamTime(DateTime utcDateTime)
		{
			if (utcDateTime.Kind == DateTimeKind.Unspecified)
				utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

			return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, VietnamTimeZone);
		}

		/// <summary>
		/// Chuyển từ giờ Việt Nam sang UTC
		/// </summary>
		public static DateTime ToUtcFromVietnam(DateTime vietnamTime)
		{
			if (vietnamTime.Kind == DateTimeKind.Unspecified)
				vietnamTime = DateTime.SpecifyKind(vietnamTime, DateTimeKind.Local);

			return TimeZoneInfo.ConvertTimeToUtc(vietnamTime, VietnamTimeZone);
		}
	}
}
