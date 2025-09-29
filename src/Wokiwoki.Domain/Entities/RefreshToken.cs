namespace Wokiwoki.Domain.Entities
{
	[Table("refresh_token")]
	public class RefreshToken : BaseAuditableEntity
	{ 
		public string UserId { get; set; }= null!;

		public string Token { get; set; } = null!;
		 
		public DateTime ExpiresAt { get; set; }

		public bool Revoked { get; set; }

		public bool IsActive => !Revoked && DateTime.UtcNow < ExpiresAt;
	}
}
