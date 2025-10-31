using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Domain.Enums;
using Wokiwoki.Infrastructure.Data.Configurations;

namespace Wokiwoki.Infrastructure.Services
{
	public class BlobStorageService : IBlobStorageService
	{
		private readonly BlobServiceClient _blobServiceClient;
		private readonly AzureBlobStorageOptions _options;
		private readonly ILogger<BlobStorageService> _logger;

		private readonly Dictionary<BlobContainerType, string> _containerMapping = new()
		{
			{ BlobContainerType.OrganizationLogos, "organizer-logos" },
			{ BlobContainerType.WorkshopMedia, "workshop-media" },
			{ BlobContainerType.Default, "default" }
		};

		public BlobStorageService(
		BlobServiceClient blobServiceClient,
		IOptions<AzureBlobStorageOptions> options,
		ILogger<BlobStorageService> logger)
		{
			_blobServiceClient = blobServiceClient;
			_options = options.Value;
			_logger = logger;
		}

		public async Task<List<string>> UploadFilesAsync(List<(Stream stream, string fileName, string contentType, long fileSize)> files, BlobContainerType containerType)
		{
			if (files == null || files.Count == 0)
				throw new ArgumentException("No files provided");
			var uploadTasks = files.Select(file =>
			UploadFileAsync(
				file.stream,
				file.fileName,
				file.contentType,
				file.fileSize,
				containerType
				)
			);

			try
			{
				var urls = await Task.WhenAll(uploadTasks);
				_logger.LogInformation(
					"Successfully uploaded {Count} files to {Container}",
					files.Count,
					_containerMapping[containerType]);

				return urls.ToList();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex,
					"Error uploading multiple files to {Container}",
					_containerMapping[containerType]);
				throw;
			}
		}

		public async Task<string> UploadFileAsync(
			Stream stream,
			string fileName,
			string contentType,
			long fileSize,
			BlobContainerType containerType)
		{
			var containerName = _containerMapping[containerType];

			try
			{
				ValidateFile(fileName, fileSize, containerName);

				var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
				var containerConfig = GetContainerConfig(containerName);

				await containerClient.CreateIfNotExistsAsync(
					containerConfig.AccessType,
					containerConfig.Metadata);

				var blobName = GenerateBlobName(fileName, containerName);
				var blobClient = containerClient.GetBlobClient(blobName);

				var blobHttpHeaders = new BlobHttpHeaders
				{
					ContentType = contentType,
					CacheControl = GetCacheControlByContainer(containerName)
				};

				var metadata = new Dictionary<string, string>
		{
			{ "OriginalFileName", fileName },
			{ "UploadedAt", DateTime.UtcNow.ToString("O") },
			{ "FileSize", fileSize.ToString() },
			{ "Container", containerName }
		};

				stream.Position = 0;
				await blobClient.UploadAsync(stream, new BlobUploadOptions
				{
					HttpHeaders = blobHttpHeaders,
					Metadata = metadata
				});

				_logger.LogInformation(
					"File uploaded successfully: {BlobName} to container: {Container}",
					blobName, containerName);

				return blobClient.Uri.ToString();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error uploading file: {FileName}", fileName);
				throw;
			}
		}

		private void ValidateFile(string fileName, long fileSize, string containerName)
		{
			if (string.IsNullOrEmpty(fileName) || fileSize == 0)
				throw new ArgumentException("File is invalid");

			var config = GetContainerConfig(containerName);

			var fileSizeInMB = fileSize / (1024.0 * 1024.0);
			if (fileSizeInMB > config.MaxFileSizeInMB)
				throw new ArgumentException($"File size exceeds {config.MaxFileSizeInMB}MB");

			if (config.AllowedFileTypes.Length > 0)
			{
				var extension = Path.GetExtension(fileName).ToLowerInvariant();
				if (!config.AllowedFileTypes.Contains(extension))
					throw new ArgumentException($"File type {extension} not allowed");
			}
		}

		private BlobConfig GetContainerConfig(string containerName)
		{
			if (_options.Containers.TryGetValue(containerName, out var config))
				return config;

			// Return default config if not found
			return new BlobConfig
			{
				Name = containerName,
				AccessType = PublicAccessType.None,
				MaxFileSizeInMB = 10,
				AllowedFileTypes = Array.Empty<string>()
			};
		}

		private string GenerateBlobName(string originalFileName, string containerName)
		{
			var extension = Path.GetExtension(originalFileName);
			var prefix = GetPrefixByContainer(containerName);
			var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");

			return $"{prefix}/{timestamp}/{Guid.NewGuid()}{extension}";
		}

		private string GetPrefixByContainer(string containerName)
		{
			return containerName switch
			{
				"organizer-logos" => "organizerlogos",
				"workshop-media" => "workshopmedias",
				"videos" => "media",
				"temp" => "temp",
				"public-assets" => "assets",
				_ => "misc"
			};
		}

		private string GetCacheControlByContainer(string containerName)
		{
			return containerName switch
			{
				"organizer-logos" => "public, max-age=31536000", // 1 year - ít thay đổi
				"workshop-media" => "public, max-age=2592000",   // 30 days
				"videos" => "public, max-age=2592000",           // 30 days
				"public-assets" => "public, max-age=31536000",   // 1 year
				"temp" => "no-cache, no-store, must-revalidate", // không cache
				_ => "public, max-age=3600"                      // 1 hour default
			};
		}
	}
}
