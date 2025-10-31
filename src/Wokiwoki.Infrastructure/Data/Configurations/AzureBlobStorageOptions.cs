namespace Wokiwoki.Infrastructure.Data.Configurations
{
	public class AzureBlobStorageOptions
	{
		public string ConnectionString { get; set; } = string.Empty;
		public string DefaultContainer { get; set; } = "default";
		public Dictionary<string, BlobConfig> Containers { get; set; } = new();
	}
}
