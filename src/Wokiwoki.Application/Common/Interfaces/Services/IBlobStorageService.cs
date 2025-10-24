using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Common.Interfaces.Services
{
	public interface IBlobStorageService
	{
		Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, long fileSize, BlobContainerType containerType);

		Task<List<string>> UploadFilesAsync(
		List<(Stream stream, string fileName, string contentType, long fileSize)> files,
		BlobContainerType containerType);
	}

	 
}
