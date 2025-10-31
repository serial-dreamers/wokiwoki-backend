using System.ComponentModel.DataAnnotations;

namespace Wokiwoki.Application.DTOs
{

	public class ChatMessageDto
	{
		[Required]
		public string Role { get; set; } = string.Empty;  // "user" or "assistant"

		[Required]
		public string Content { get; set; } = string.Empty;
	}

	public class ChatResponse
	{
		[Required]
		public string Message { get; set; } = string.Empty;

		[Required]
		public string Markdown { get; set; } = string.Empty;
	}

}
