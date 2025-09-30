using Medo;
using Microsoft.EntityFrameworkCore;

namespace Wokiwoki.Infrastructure.Data.Seeder
{
	public class ApplicationTagSeeder
	{
		public static async Task SeedAsync(WokiwokiDbContext context)
		{
			var systemUserId = "00000000-0000-0000-0000-000000000001";
			DateTime now = DateTime.UtcNow;

			var tags = new List<Tag>
		{
			// Tags cho Nghệ thuật & Thủ công (10 tags)
			new Tag { Id = Uuid7.NewUuid7(), Name = "Vẽ tranh", Description = "Kỹ thuật vẽ tranh các loại", IconUrl = "https://example.com/icons/painting.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Origami", Description = "Nghệ thuật gấp giấy Nhật Bản", IconUrl = "https://example.com/icons/origami.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Thêu thùa", Description = "Nghệ thuật thêu và may vá", IconUrl = "https://example.com/icons/embroidery.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Đan len", Description = "Kỹ thuật đan len và móc len", IconUrl = "https://example.com/icons/knitting.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Làm đồ gốm", Description = "Nghệ thuật tạo hình và nung gốm", IconUrl = "https://example.com/icons/pottery.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Thủ công DIY", Description = "Các dự án tự làm sáng tạo", IconUrl = "https://example.com/icons/diy.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Calligraphy", Description = "Nghệ thuật viết chữ đẹp", IconUrl = "https://example.com/icons/calligraphy.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Làm trang sức", Description = "Thiết kế và chế tác trang sức handmade", IconUrl = "https://example.com/icons/jewelry.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Làm nến thơm", Description = "Nghệ thuật làm nến và tinh dầu", IconUrl = "https://example.com/icons/candle.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Xếp hình 3D", Description = "Mô hình giấy và 3D papercraft", IconUrl = "https://example.com/icons/3dcraft.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },

			// Tags cho Ẩm thực & Nấu ăn (10 tags)
			new Tag { Id = Uuid7.NewUuid7(), Name = "Món Việt", Description = "Các món ăn truyền thống Việt Nam", IconUrl = "https://example.com/icons/vietnamese.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Món Âu", Description = "Ẩm thực phương Tây", IconUrl = "https://example.com/icons/western.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Món Á", Description = "Ẩm thực châu Á đa dạng", IconUrl = "https://example.com/icons/asian.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Làm bánh", Description = "Kỹ thuật làm bánh ngọt và bánh mì", IconUrl = "https://example.com/icons/baking.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Ăn chay", Description = "Món ăn thuần chay và healthy", IconUrl = "https://example.com/icons/vegan.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Pha chế đồ uống", Description = "Cà phê, trà và cocktail", IconUrl = "https://example.com/icons/beverages.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Món ăn nhanh", Description = "Fast food và street food", IconUrl = "https://example.com/icons/fastfood.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Trang trí món ăn", Description = "Food styling và plating", IconUrl = "https://example.com/icons/plating.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Nấu ăn gia đình", Description = "Món ăn hàng ngày cho cả nhà", IconUrl = "https://example.com/icons/homecooking.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Làm tương ớt & gia vị", Description = "Chế biến gia vị tự nhiên", IconUrl = "https://example.com/icons/spices.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },

			// Tags cho Kỹ năng số & Công nghệ (10 tags)
			new Tag { Id = Uuid7.NewUuid7(), Name = "Lập trình", Description = "Học các ngôn ngữ lập trình", IconUrl = "https://example.com/icons/coding.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "AI & Machine Learning", Description = "Trí tuệ nhân tạo và học máy", IconUrl = "https://example.com/icons/ai.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Thiết kế đồ họa", Description = "Photoshop, Illustrator, Figma", IconUrl = "https://example.com/icons/design.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Marketing online", Description = "Digital marketing và SEO", IconUrl = "https://example.com/icons/marketing.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "An ninh mạng", Description = "Bảo mật thông tin và hệ thống", IconUrl = "https://example.com/icons/security.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Dựng video", Description = "Chỉnh sửa và dựng video chuyên nghiệp", IconUrl = "https://example.com/icons/video.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Phát triển web", Description = "Frontend, Backend và Full-stack", IconUrl = "https://example.com/icons/webdev.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Phát triển App", Description = "Mobile app iOS và Android", IconUrl = "https://example.com/icons/mobile.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Data Science", Description = "Phân tích dữ liệu và Big Data", IconUrl = "https://example.com/icons/data.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "UX/UI Design", Description = "Thiết kế trải nghiệm người dùng", IconUrl = "https://example.com/icons/uxui.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },

			// Tags cho Ngôn ngữ & Văn hóa (10 tags)
			new Tag { Id = Uuid7.NewUuid7(), Name = "Tiếng Anh", Description = "Học và giao tiếp tiếng Anh", IconUrl = "https://example.com/icons/english.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Tiếng Nhật", Description = "Ngôn ngữ và văn hóa Nhật Bản", IconUrl = "https://example.com/icons/japanese.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Tiếng Hàn", Description = "K-culture và tiếng Hàn Quốc", IconUrl = "https://example.com/icons/korean.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Tiếng Trung", Description = "Mandarin và văn hóa Trung Quốc", IconUrl = "https://example.com/icons/chinese.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Văn hóa dân gian", Description = "Truyền thống và phong tục Việt Nam", IconUrl = "https://example.com/icons/culture.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Du lịch & Khám phá", Description = "Văn hóa các quốc gia trên thế giới", IconUrl = "https://example.com/icons/travel.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Tiếng Pháp", Description = "La langue française et sa culture", IconUrl = "https://example.com/icons/french.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Tiếng Tây Ban Nha", Description = "Español y cultura hispana", IconUrl = "https://example.com/icons/spanish.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "IELTS/TOEIC", Description = "Luyện thi chứng chỉ quốc tế", IconUrl = "https://example.com/icons/ielts.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Lịch sử thế giới", Description = "Tìm hiểu lịch sử và văn minh", IconUrl = "https://example.com/icons/history.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },

			// Tags cho Sức khỏe & Đời sống (10 tags)
			new Tag { Id = Uuid7.NewUuid7(), Name = "Yoga", Description = "Thực hành yoga cho sức khỏe", IconUrl = "https://example.com/icons/yoga.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Gym & Fitness", Description = "Tập luyện và xây dựng cơ bắp", IconUrl = "https://example.com/icons/fitness.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Thiền định", Description = "Mindfulness và sức khỏe tâm thần", IconUrl = "https://example.com/icons/meditation.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Dinh dưỡng", Description = "Chế độ ăn uống khoa học", IconUrl = "https://example.com/icons/nutrition.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Chạy bộ", Description = "Running và marathon", IconUrl = "https://example.com/icons/running.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Làm đẹp", Description = "Chăm sóc da và trang điểm", IconUrl = "https://example.com/icons/beauty.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Pilates", Description = "Bài tập cải thiện vóc dáng", IconUrl = "https://example.com/icons/pilates.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Võ thuật", Description = "Karate, Taekwondo, Muay Thai", IconUrl = "https://example.com/icons/martial.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Massage & Spa", Description = "Kỹ thuật massage và thư giãn", IconUrl = "https://example.com/icons/massage.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Sống xanh", Description = "Lối sống bền vững và thân thiện môi trường", IconUrl = "https://example.com/icons/green.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },

			// Tags cho Kinh doanh & Khởi nghiệp (10 tags)
			new Tag { Id = Uuid7.NewUuid7(), Name = "Khởi nghiệp", Description = "Xây dựng startup từ con số 0", IconUrl = "https://example.com/icons/startup.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Quản trị doanh nghiệp", Description = "Kỹ năng quản lý và điều hành", IconUrl = "https://example.com/icons/management.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Tài chính cá nhân", Description = "Đầu tư và quản lý tài chính", IconUrl = "https://example.com/icons/finance.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Bán hàng", Description = "Kỹ năng sales và chăm sóc khách hàng", IconUrl = "https://example.com/icons/sales.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "E-commerce", Description = "Kinh doanh trực tuyến", IconUrl = "https://example.com/icons/ecommerce.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Kế toán", Description = "Kế toán và báo cáo tài chính", IconUrl = "https://example.com/icons/accounting.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Đầu tư chứng khoán", Description = "Trading và phân tích thị trường", IconUrl = "https://example.com/icons/stock.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Bất động sản", Description = "Đầu tư và kinh doanh BDS", IconUrl = "https://example.com/icons/realestate.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Social Media", Description = "Quản lý và phát triển mạng xã hội", IconUrl = "https://example.com/icons/socialmedia.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Nhượng quyền thương hiệu", Description = "Franchise và mở rộng kinh doanh", IconUrl = "https://example.com/icons/franchise.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },

			// Tags cho Trải nghiệm & Giải trí (10 tags)
			new Tag { Id = Uuid7.NewUuid7(), Name = "Âm nhạc", Description = "Học nhạc cụ và lý thuyết âm nhạc", IconUrl = "https://example.com/icons/music.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Nhiếp ảnh", Description = "Kỹ thuật chụp và xử lý ảnh", IconUrl = "https://example.com/icons/photography.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Điện ảnh", Description = "Phân tích phim và làm phim", IconUrl = "https://example.com/icons/cinema.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Gaming", Description = "Game và esports", IconUrl = "https://example.com/icons/gaming.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Sách & Đọc", Description = "Review sách và thói quen đọc", IconUrl = "https://example.com/icons/books.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Sân khấu", Description = "Kịch nghệ và biểu diễn", IconUrl = "https://example.com/icons/theater.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Nhảy múa", Description = "Dance covers và vũ đạo", IconUrl = "https://example.com/icons/dance.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Magic & Ảo thuật", Description = "Trình diễn và học ảo thuật", IconUrl = "https://example.com/icons/magic.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Podcast & Streaming", Description = "Tạo nội dung audio và video", IconUrl = "https://example.com/icons/podcast.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Board games", Description = "Board game và trò chơi chiến thuật", IconUrl = "https://example.com/icons/boardgame.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },

			// Tags cho Giao tiếp & Kỹ năng mềm (8 tags)
			new Tag { Id = Uuid7.NewUuid7(), Name = "Thuyết trình", Description = "Kỹ năng public speaking", IconUrl = "https://example.com/icons/presentation.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Làm việc nhóm", Description = "Teamwork và collaboration", IconUrl = "https://example.com/icons/teamwork.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Lãnh đạo", Description = "Leadership và tư duy quản lý", IconUrl = "https://example.com/icons/leadership.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Đàm phán", Description = "Kỹ thuật thương lượng", IconUrl = "https://example.com/icons/negotiation.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Quản lý thời gian", Description = "Tăng năng suất làm việc", IconUrl = "https://example.com/icons/time.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Tư duy phản biện", Description = "Critical thinking và giải quyết vấn đề", IconUrl = "https://example.com/icons/thinking.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Emotional Intelligence", Description = "Trí tuệ cảm xúc và EQ", IconUrl = "https://example.com/icons/eq.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },
			new Tag { Id = Uuid7.NewUuid7(), Name = "Networking", Description = "Kỹ năng xây dựng mối quan hệ", IconUrl = "https://example.com/icons/networking.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now },

			// Orther
			new Tag { Id = Uuid7.NewUuid7(), Name = "Khác", Description = "Khám phá nhiều thể loại khác nhau", IconUrl = "https://example.com/icons/networking.png", CreatedBy = systemUserId, Created = now, LastModifiedBy = systemUserId, LastModified = now }
		};

			foreach (var tag in tags)
			{
				bool exists = await context.Tags
					.AnyAsync(t => t.Name!.ToLower() == tag.Name!.ToLower());
				if (!exists)
				{
					await context.Tags.AddAsync(tag);
				}
			}

			await context.SaveChangesAsync();
		}
	}
}
