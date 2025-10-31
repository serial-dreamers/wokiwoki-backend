using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Wokiwoki.Application.Common.Interfaces.Repositories;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Infrastructure.Repositories
{
	public class WorkshopHeroMediaRepository : BaseRepo<WorkshopHeroMedia, Guid>, IWorkshopHeroMediaRepository
	{
		private const int MAX_MAIN_IMAGES = 4;
		private const int MAX_VIDEOS = 1;
		private const int MAX_SLIDER_IMAGES = 10;

		public WorkshopHeroMediaRepository(WokiwokiDbContext context) : base(context)
		{
		}

		public async Task<List<WorkshopHeroMedia>> GetHeroMediasByWorkshopId(Guid workshopId)
		{
			return await _context.WorkshopHeroMedias.Include(hrm => hrm.WorkshopMedia).Where(hrm => hrm.WorkshopId == workshopId && hrm.IsActive == true).ToListAsync();	  
		}

		public async Task SyncHeroMediaAsync(
		Guid workshopId,
		List<WorkshopHeroMediaDto> heroMediaDtos,
		CancellationToken cancellationToken = default)
		{
			// Validation rules
			var mainImageCount = heroMediaDtos.Count(x => x.HeroType == HeroMediaType.MainImage);
			var videoCount = heroMediaDtos.Count(x => x.HeroType == HeroMediaType.Video);
			var sliderCount = heroMediaDtos.Count(x => x.HeroType == HeroMediaType.SliderImage);

			if (mainImageCount > MAX_MAIN_IMAGES)
				throw new ValidationException($"Maximum {MAX_MAIN_IMAGES} Main Images allowed");
			if (videoCount > MAX_VIDEOS)
				throw new ValidationException($"Maximum {MAX_VIDEOS} Video allowed");
			if (sliderCount > MAX_SLIDER_IMAGES)
				throw new ValidationException($"Maximum {MAX_SLIDER_IMAGES} Slider Images allowed");

			// Lấy tất cả hero media hiện tại của workshop
			var existingHeroMedias = await _context.Set<WorkshopHeroMedia>()
				.Where(x => x.WorkshopId == workshopId)
				.ToListAsync(cancellationToken);

			// Tách ra theo type để xử lý riêng
			var existingMainImages = existingHeroMedias
				.Where(x => x.HeroType == HeroMediaType.MainImage)
				.ToList();
			var existingVideos = existingHeroMedias
				.Where(x => x.HeroType == HeroMediaType.Video)
				.ToList();
			var existingSliders = existingHeroMedias
				.Where(x => x.HeroType == HeroMediaType.SliderImage)
				.ToList();

			// Xử lý Main Images (tối đa 4)
			var newMainImages = heroMediaDtos
				.Where(x => x.HeroType == HeroMediaType.MainImage)
				.ToList();

			await SyncMediaByTypeAsync(
				workshopId,
				existingMainImages,
				newMainImages,
				HeroMediaType.MainImage,
				cancellationToken);

			// Xử lý Video (tối đa 1)
			var newVideos = heroMediaDtos
				.Where(x => x.HeroType == HeroMediaType.Video)
				.ToList();

			await SyncMediaByTypeAsync(
				workshopId,
				existingVideos,
				newVideos,
				HeroMediaType.Video,
				cancellationToken);

			// Xử lý Slider Images (tối đa 10)
			var newSliders = heroMediaDtos
				.Where(x => x.HeroType == HeroMediaType.SliderImage)
				.ToList();

			await SyncMediaByTypeAsync(
				workshopId,
				existingSliders,
				newSliders,
				HeroMediaType.SliderImage,
				cancellationToken);

			await _context.SaveChangesAsync(cancellationToken);
		}

		private async Task SyncMediaByTypeAsync(
			Guid workshopId,
			List<WorkshopHeroMedia> existingMedias,
			List<WorkshopHeroMediaDto> newMediaDtos,
			HeroMediaType mediaType,
			CancellationToken cancellationToken)
		{
			// 1. Xác định các item cần XÓA (soft delete)
			// Những item có trong DB nhưng KHÔNG có trong request
			var newMediaIds = newMediaDtos
				.Where(x => x.MediaId.HasValue)
				.Select(x => x.MediaId.Value)
				.ToHashSet();

			var itemsToDelete = existingMedias
				.Where(x => x.MediaId.HasValue && !newMediaIds.Contains(x.MediaId.Value))
				.ToList();

			foreach (var item in itemsToDelete)
			{
				item.IsActive = false; // Soft delete
				item.LastModified = DateTime.UtcNow;
			}

			// 2. Xác định các item cần UPDATE
			// Những item có MediaId trùng nhau
			foreach (var newDto in newMediaDtos.Where(x => x.MediaId.HasValue))
			{
				var existingItem = existingMedias
					.FirstOrDefault(x => x.MediaId == newDto.MediaId && x.IsActive);

				if (existingItem != null)
				{
					// Update existing item
					existingItem.IsActive = true;
					existingItem.LastModified = DateTime.UtcNow;
					// Note: HeroType và WorkshopId không thay đổi
				}
			}

			// 3. Xác định các item cần CREATE
			// Những MediaId mới chưa tồn tại trong DB (hoặc đã bị soft delete)
			var existingMediaIds = existingMedias
				.Where(x => x.MediaId.HasValue && x.IsActive)
				.Select(x => x.MediaId.Value)
				.ToHashSet();

			var itemsToCreate = newMediaDtos
				.Where(x => x.MediaId.HasValue && !existingMediaIds.Contains(x.MediaId.Value))
				.ToList();

			foreach (var dto in itemsToCreate)
			{
				// Check xem có item bị soft delete với MediaId này không
				var softDeletedItem = existingMedias
					.FirstOrDefault(x => x.MediaId == dto.MediaId && !x.IsActive);

				if (softDeletedItem != null)
				{
					// Reactivate item đã bị soft delete
					softDeletedItem.IsActive = true;
					softDeletedItem.LastModified = DateTime.UtcNow;
				}
				else
				{
					// Create new item
					var newHeroMedia = new WorkshopHeroMedia
					{
						Id = Guid.NewGuid(),
						WorkshopId = workshopId,
						MediaId = dto.MediaId,
						HeroType = mediaType,
						IsActive = true,
						Created = DateTime.UtcNow
					};

					await _context.Set<WorkshopHeroMedia>().AddAsync(newHeroMedia, cancellationToken);
				}
			}
		}
	}
}
