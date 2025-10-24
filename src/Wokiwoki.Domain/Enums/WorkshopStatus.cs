namespace Wokiwoki.Domain.Enums
{
	public enum WorkshopStatus
	{
		/// <summary>
		/// Người tổ chức mới tạo, chưa công khai
		/// </summary>
		Draft = 0,

		/// <summary>
		/// Đã gửi lên chờ duyệt (nếu bạn có hệ thống kiểm duyệt)
		/// </summary>
		PendingReview = 1,

		/// <summary>
		/// Đã được public ra ngoài trang chủ / tìm kiếm
		/// </summary>
		Published = 2,

		/// <summary>
		/// Ẩn tạm thời (organizer hoặc admin ẩn)
		/// </summary>
		Hidden = 3,

		/// <summary>
		/// Sự kiện bị hủy
		/// </summary>
		Cancelled = 4
	}
}
