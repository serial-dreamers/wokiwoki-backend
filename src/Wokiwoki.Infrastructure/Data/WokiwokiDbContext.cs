using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text.Json;

namespace Wokiwoki.Infrastructure.Data
{
	public class WokiwokiDbContext : IdentityDbContext<ApplicationUser>
	{
		// === DbSet ===
		public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
		public DbSet<Booking> Bookings => Set<Booking>();
		public DbSet<Category> Categories => Set<Category>();
		public DbSet<Organization> Organizations => Set<Organization>();
		public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
		public DbSet<Tag> Tags => Set<Tag>();
		public DbSet<Ticket> Tickets => Set<Ticket>();
		public DbSet<UserTagPreference> UserTagPreferences => Set<UserTagPreference>();
		public DbSet<UserWorkshopLike> UserWorkshopLikes => Set<UserWorkshopLike>();
		public DbSet<UserOrganizationFollow> UserOrganizationFollows => Set<UserOrganizationFollow>();
		public DbSet<Workshop> Workshops => Set<Workshop>();
		public DbSet<WorkshopSchedule> WorkshopSchedules => Set<WorkshopSchedule>();
		public DbSet<WorkshopScheduleTicket> WorkshopScheduleTickets => Set<WorkshopScheduleTicket>();
		public DbSet<WorkshopSession> WorkshopSessions => Set<WorkshopSession>();
		public DbSet<WorkshopHeroMedia> WorkshopHeroMedias => Set<WorkshopHeroMedia>();
		public DbSet<WorkshopMedia> WorkshopMedias => Set<WorkshopMedia>();
		public DbSet<Review> Reviews => Set<Review>();
		public DbSet<ConversationChat> ConversationChats => Set<ConversationChat>();
		public DbSet<MessageChat> MessageChats => Set<MessageChat>();

		private readonly string _currentUser;

		public WokiwokiDbContext()
		{
		}

		public WokiwokiDbContext(DbContextOptions<WokiwokiDbContext> options, IHttpContextAccessor httpContextAccessor)
			: base(options)
		{
			_currentUser = httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			ApplyLowercaseNaming(modelBuilder);

			ConfigureAuditLog(modelBuilder);
			ConfigureBooking(modelBuilder);	
			ConfigureOrganization(modelBuilder);
			ConfigureRefreshToken(modelBuilder);
			ConfigureTag(modelBuilder);
			ConfigureUserTagPreference(modelBuilder);
			ConfigureUserWorkshopLike(modelBuilder);
			ConfigureUserOrganizationFollow(modelBuilder);
			ConfigureWorkshop(modelBuilder);
			ConfigureWorkshopSchedule(modelBuilder);
			ConfigureWorkshopScheduleTicket(modelBuilder);
			ConfigureWorkshopSession(modelBuilder);
			ConfigureWorkshopMedia(modelBuilder);
			ConfigureWorkshopHeroMedia(modelBuilder);
			ConfigureTicket(modelBuilder);
			ConfigureReview(modelBuilder);
			ConfigureTagRelationships(modelBuilder);
			ConfigureConversationChat(modelBuilder);
			ConfigureMessageChat(modelBuilder);
		}

		// =====================================================================
		// ====================== CONFIGURATION DETAILS =========================
		// =====================================================================

		private static void ApplyLowercaseNaming(ModelBuilder modelBuilder)
		{
			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				entityType.SetTableName(entityType.GetTableName()?.ToLower());

				foreach (var property in entityType.GetProperties())
				{
					var columnName = property.GetColumnName(StoreObjectIdentifier.Table(entityType.GetTableName()!, entityType.GetSchema()));
					property.SetColumnName(columnName!.ToLower());

					if (property.ClrType == typeof(Guid) || property.ClrType == typeof(Guid?))
						property.SetColumnType("uuid");
				}

				foreach (var key in entityType.GetKeys())
					key.SetName(key.GetName()!.ToLower());

				foreach (var fk in entityType.GetForeignKeys())
					fk.SetConstraintName(fk.GetConstraintName()!.ToLower());

				foreach (var index in entityType.GetIndexes())
					index.SetDatabaseName(index.GetDatabaseName()!.ToLower());
			}
		}

		private static void ConfigureAuditLog(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<AuditLog>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Action).HasMaxLength(50);
				entity.Property(e => e.EntityName).HasMaxLength(100);
				entity.Property(e => e.OriginalValue).HasColumnType("text");
				entity.Property(e => e.NewValue).HasColumnType("text");

				entity.HasIndex(e => new { e.EntityName, e.Created });
				entity.HasIndex(e => e.LastModifiedBy);
			});
		}

		private static void ConfigureBooking(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Booking>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.TotalPrice).HasColumnType("numeric(18,2)");

				entity.HasOne(e => e.Workshop)
					.WithMany()
					.HasForeignKey(e => e.WorkshopId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasIndex(e => e.UserId);
				entity.HasIndex(e => e.WorkshopId);

				entity.HasMany(e => e.Tickets)
					.WithOne(t => t.Booking)
					.HasForeignKey(t => t.BookingId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasMany(e => e.Reviews)
					.WithOne(t => t.Booking)
					.HasForeignKey(t => t.BookingId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}

		private static void ConfigureOrganization(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Organization>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
				entity.Property(e => e.Description).HasColumnType("text");
				entity.Property(e => e.LogoUrl).HasMaxLength(500);
				entity.Property(e => e.ContactEmail).HasMaxLength(255);
				entity.Property(e => e.ContactPhone).HasMaxLength(20);
				entity.Property(e => e.Street).HasMaxLength(255);
				entity.Property(e => e.Commune).HasMaxLength(100);
				entity.Property(e => e.Province).HasMaxLength(100);
			});
		}

		private static void ConfigureRefreshToken(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<RefreshToken>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Token).HasMaxLength(500).IsRequired();
				entity.Ignore(e => e.IsActive);

				entity.HasIndex(e => e.UserId);
				entity.HasIndex(e => e.Token);
			});
		}

		private static void ConfigureTag(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Tag>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).HasMaxLength(100);
				entity.Property(e => e.Description).HasMaxLength(500);
				entity.Property(e => e.IconUrl).HasMaxLength(500);
			});
		}

		private static void ConfigureUserTagPreference(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<UserTagPreference>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.HasIndex(e => new { e.UserId, e.TagId, e.CategoryId }).IsUnique();

				entity.HasOne(e => e.Tag)
					.WithMany(e => e.UserPreferences)
					.HasForeignKey(e => e.TagId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Category)
					.WithMany(e => e.TagPreferences)
					.HasForeignKey(e => e.CategoryId)
					.OnDelete(DeleteBehavior.Restrict);
			});
		}

		private static void ConfigureUserWorkshopLike(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<UserWorkshopLike>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.HasIndex(e => new { e.UserId, e.WorkshopId }).IsUnique();

				entity.HasOne(e => e.Workshop)
					.WithMany(e => e.Likes)
					.HasForeignKey(e => e.WorkshopId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}

		private static void ConfigureUserOrganizationFollow(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<UserOrganizationFollow>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.HasIndex(e => new { e.UserId, e.OrganizationId }).IsUnique();

				entity.HasOne(e => e.Organization)
					.WithMany(e => e.Followers)
					.HasForeignKey(e => e.OrganizationId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}

		private static void ConfigureWorkshop(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Workshop>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Title).HasMaxLength(100).IsRequired();
				entity.Property(e => e.Summary).HasMaxLength(150).IsRequired();
				entity.Property(e => e.Description).HasColumnType("text").IsRequired();
				entity.Property(e => e.ImageUrl).HasMaxLength(500);
				entity.Property(e => e.DisplayAddress).HasMaxLength(255);
				entity.Property(e => e.Latitude).HasColumnType("numeric(10,7)");
				entity.Property(e => e.Longitude).HasColumnType("numeric(10,7)");
				entity.Property(e => e.OnlineEventUrl).HasMaxLength(500);
				entity.Property(e => e.StartingPrice).HasColumnType("numeric(18,2)");
				entity.Property(e => e.RefundPolicyDescription).HasMaxLength(1000);

				entity.HasOne(e => e.Organization)
					.WithMany()
					.HasForeignKey(e => e.OrganizationId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Category)
					.WithMany(e => e.Workshops)
					.HasForeignKey(e => e.CategoryId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasIndex(e => e.OrganizationId);
				entity.HasIndex(e => e.CategoryId);
			});
		}

		private static void ConfigureWorkshopSchedule(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<WorkshopSchedule>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.DaysOfWeek).HasMaxLength(50);
				entity.Property(e => e.DaysOfMonth).HasMaxLength(100);

				entity.HasOne(e => e.Workshop)
					.WithMany(w => w.Schedules)
					.HasForeignKey(e => e.WorkshopId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasIndex(e => e.WorkshopId);
				entity.HasIndex(e => new { e.WorkshopId, e.RecurrenceType });
			});
		}

		private static void ConfigureWorkshopScheduleTicket(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<WorkshopScheduleTicket>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
				entity.Property(e => e.Price).HasColumnType("numeric(18,2)").IsRequired();

				entity.HasOne(e => e.WorkshopSchedule)
					.WithMany(ws => ws.Tickets)
					.HasForeignKey(e => e.WorkshopScheduleId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasIndex(e => e.WorkshopScheduleId);
				entity.HasIndex(e => e.IsActive);
			});
		}

		private static void ConfigureWorkshopSession(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<WorkshopSession>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Title).HasMaxLength(255).IsRequired();
				entity.Property(e => e.Description).HasColumnType("text").IsRequired();
				entity.Property(e => e.Street).HasMaxLength(255);
				entity.Property(e => e.Commune).HasMaxLength(100);
				entity.Property(e => e.Province).HasMaxLength(100);
				entity.Property(e => e.Latitude).HasColumnType("numeric(10,7)");
				entity.Property(e => e.Longitude).HasColumnType("numeric(10,7)");
				entity.Property(e => e.ParkingDescription).HasMaxLength(500);

				entity.HasOne(e => e.Workshop)
					.WithMany(e => e.WorkshopSessions)
					.HasForeignKey(e => e.WorkshopId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne<WorkshopSchedule>()
					.WithMany(s => s.Sessions)
					.HasForeignKey(e => e.ScheduleId)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasMany(e => e.Tickets)
					.WithOne(e => e.WorkshopSession)
					.HasForeignKey(e => e.SessionId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasIndex(e => e.WorkshopId);
				entity.HasIndex(e => e.ScheduleId);
				entity.HasIndex(e => new { e.StartTime, e.IsActive });
				entity.HasIndex(e => new { e.WorkshopId, e.StartTime });
			});
		}

		private static void ConfigureWorkshopMedia(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<WorkshopMedia>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.ImageUrl).HasMaxLength(500);

				entity.HasOne(e => e.Workshop)
					.WithMany(e => e.WorkshopMedias)
					.HasForeignKey(e => e.WorkshopId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}

		private static void ConfigureWorkshopHeroMedia(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<WorkshopHeroMedia>(entity =>
			{
				entity.HasKey(e => e.Id);

				entity.HasOne(e => e.Workshop)
					.WithMany(e => e.WorkshopHeroMedias)
					.HasForeignKey(e => e.WorkshopId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(e => e.WorkshopMedia)
					.WithMany()
					.HasForeignKey(e => e.MediaId)
					.OnDelete(DeleteBehavior.Restrict);
			});
		}

		private static void ConfigureTicket(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Ticket>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Price).HasColumnType("numeric(18,2)");
				entity.Property(e => e.QrCodeImage).IsRequired();
				entity.Property(e => e.Quantity).IsRequired();

				entity.HasOne(e => e.TicketType)
					.WithMany(st => st.Tickets)
					.HasForeignKey(e => e.TicketTypeId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Booking)
					.WithMany(b => b.Tickets)
					.HasForeignKey(e => e.BookingId)
					.OnDelete(DeleteBehavior.Restrict);


				entity.HasOne(e => e.WorkshopSession)
					.WithMany(s => s.Tickets)
					.HasForeignKey(e => e.SessionId)
					.OnDelete(DeleteBehavior.Restrict);

			});
		}

		private static void ConfigureReview(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Review>(entity =>
			{
				entity.HasKey(r => r.Id);
				entity.HasIndex(r => new { r.WorkshopId, r.UserId }).IsUnique();
				entity.Property(r => r.Rating).IsRequired();

				entity.HasOne(r => r.Workshop)
					.WithMany(w => w.Reviews)
					.HasForeignKey(r => r.WorkshopId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(r => r.Booking)
					.WithMany(w => w.Reviews)
					.HasForeignKey(r => r.BookingId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}

		private static void ConfigureTagRelationships(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Tag>()
				.HasMany(e => e.Categories)
				.WithMany(e => e.Tags)
				.UsingEntity("category_tag");

			modelBuilder.Entity<Tag>()
				.HasMany(e => e.Workshops)
				.WithMany(e => e.Tags)
				.UsingEntity("workshop_tag");
		}

		private static void ConfigureConversationChat(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ConversationChat>(entity =>
			{
				entity.HasKey(e => e.Id);

				entity.Property(e => e.UserId)
					.IsRequired()
					.HasMaxLength(100);

				entity.Property(e => e.Title)
					.HasMaxLength(255); 

				entity.Property(e => e.IsActive)
					.HasDefaultValue(true);

				entity.HasMany(e => e.MessagesChats)
					.WithOne(e => e.ConversationChat)
					.HasForeignKey(e => e.ConversationId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}

		private static void ConfigureMessageChat(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<MessageChat>(entity =>
			{
				entity.HasKey(e => e.Id);

				entity.Property(e => e.Role)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.Content)
					.IsRequired(); 

				entity.HasOne(e => e.ConversationChat)
					.WithMany(e => e.MessagesChats)
					.HasForeignKey(e => e.ConversationId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}

	}
}
