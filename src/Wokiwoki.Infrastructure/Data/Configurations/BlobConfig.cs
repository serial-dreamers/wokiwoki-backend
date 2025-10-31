using Azure.Storage.Blobs.Models;

namespace Wokiwoki.Infrastructure.Data.Configurations
{
	public class BlobConfig
	{
		public string Name { get; set; } = string.Empty;
		public PublicAccessType AccessType { get; set; } = PublicAccessType.None;
		public long MaxFileSizeInMB { get; set; } = 10;
		public string[] AllowedFileTypes { get; set; } = Array.Empty<string>();
		public Dictionary<string, string> Metadata { get; set; } = new();
	}
}
