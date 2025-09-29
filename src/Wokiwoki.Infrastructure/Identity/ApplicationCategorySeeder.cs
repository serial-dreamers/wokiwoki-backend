using Medo;
using Microsoft.EntityFrameworkCore; 
namespace Wokiwoki.Infrastructure.Identity
{
	public class ApplicationCategorySeeder
	{
		public static async Task SeedAsync(WokiwokiDbContext context)
		{
			var systemUserId =  "00000000-0000-0000-0000-000000000001";
			DateTime now = DateTime.UtcNow;
			var categories = new List<Category>
			{
				new Category {Id=Uuid7.NewUuid7(), Name = "Nghệ thuật & Thủ công", Description = "Khám phá các kỹ thuật sáng tạo và hoạt động thủ công nghệ thuật.", IconUrl = "https://example.com/icons/technology.png", ImageUrl = "https://wokiwokistorageea.blob.core.windows.net/qrticket-image/nghethuatvathucong.jpg", CreatedBy=systemUserId, Created=now, LastModifiedBy=systemUserId, LastModified =now  },

				new Category {Id=Uuid7.NewUuid7(), Name = "Ẩm thực & Nấu ăn", Description = "Công thức món ăn ngon và bí quyết nấu nướng hàng ngày.", IconUrl = "https://example.com/icons/health.png", ImageUrl = "https://media.istockphoto.com/id/516329534/photo/last-straw.jpg?s=612x612&w=0&k=20&c=q9tScD01SPtN5QNAYgWG-ot4n_4hZXOgMStuFgmBFa8=",  CreatedBy=systemUserId, Created=now, LastModifiedBy=systemUserId, LastModified =now },

				new Category {Id=Uuid7.NewUuid7(), Name = "Kỹ năng số & Công nghệ", Description = "Cập nhật kiến thức công nghệ và kỹ năng số hiện đại.", IconUrl = "https://example.com/icons/travel.png", ImageUrl = "https://kynangso.edu.vn/wp-content/uploads/2025/04/phat-trien-ky-nang-so.webp", CreatedBy=systemUserId, Created=now, LastModifiedBy=systemUserId, LastModified =now },

				new Category {Id=Uuid7.NewUuid7(), Name = "Ngôn ngữ & Văn hóa", Description = "Học ngôn ngữ mới và tìm hiểu sự đa dạng văn hóa.", IconUrl = "https://example.com/icons/food.png", ImageUrl = "https://wpvip.edutopia.org/wp-content/uploads/2022/10/a4227ir1171-clone-crop.jpg?w=2880&quality=85", CreatedBy=systemUserId, Created=now, LastModifiedBy=systemUserId, LastModified =now },

				new Category {Id=Uuid7.NewUuid7(), Name = "Sức khỏe & Đời sống", Description = "Lời khuyên chăm sóc sức khỏe và lối sống cân bằng.", IconUrl = "https://example.com/icons/finance.png", ImageUrl = "https://vietjack.me/storage/uploads/images/34622/screenshot-2024-03-04-164658-1709545648.png", CreatedBy=systemUserId, Created=now, LastModifiedBy=systemUserId, LastModified =now },

				new Category {Id=Uuid7.NewUuid7(), Name = "Kinh doanh & Khởi nghiệp", Description = "Chiến lược kinh doanh và ý tưởng khởi nghiệp sáng tạo.", IconUrl = "https://example.com/icons/education.png", ImageUrl = "https://images.careerviet.vn/content/images/2(20).jpg", CreatedBy=systemUserId, Created=now, LastModifiedBy=systemUserId, LastModified =now },

				new Category {Id=Uuid7.NewUuid7(), Name = "Trái nghiệm & Giải trí", Description = "Phim ảnh, âm nhạc, trò chơi và các hoạt động giải trí khác.", IconUrl = "https://example.com/icons/entertainment.png", ImageUrl = "https://fox5sandiego.com/wp-content/uploads/sites/15/2023/11/BVG-Press-Photos-01-e1701239280551.jpg?w=900", CreatedBy=systemUserId, Created=now, LastModifiedBy=systemUserId, LastModified =now },

				new Category {Id=Uuid7.NewUuid7(), Name = "Giao tiếp & Kỹ năng mềm", Description = "Phát triển kỹ năng giao tiếp và kỹ năng làm việc nhóm.", IconUrl = "https://example.com/icons/sports.png", ImageUrl = "https://yourhomework.net/yhw/f/yhw-voca/2024/03/1/202403180402015404087.jpg",  CreatedBy=systemUserId, Created=now, LastModifiedBy=systemUserId, LastModified =now } 
			};

			foreach (var category in categories)
			{
				bool exists = await context.Categories
					.AnyAsync(c => c.Name!.ToLower() == category.Name!.ToLower());

				if (!exists)
				{
					await context.Categories.AddAsync(category);
				}
			}

			await context.SaveChangesAsync();
		}
	}
}
