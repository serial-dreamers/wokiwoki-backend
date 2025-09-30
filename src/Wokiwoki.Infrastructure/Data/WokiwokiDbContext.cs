using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Wokiwoki.Infrastructure.Data
{
	public class WokiwokiDbContext : IdentityDbContext<ApplicationUser>
	{

		public DbSet<AuditLog> AuditLogs { get; set; }
		public DbSet<Booking> Bookings { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Organization> Organizations { get; set; }
		public DbSet<OrganizationMember> OrganizationMembers { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<Ticket> Tickets { get; set; }
		public DbSet<UserTagPreference> UserTagPreferences { get; set; }
		public DbSet<UserWorkshopLike> UserWorkshopLikes { get; set; }
		public DbSet<Workshop> Workshops { get; set; }
		public DbSet<WorkshopHeroMedia> WorkshopHeroMedias { get; set; }
		public DbSet<WorkshopMedia> WorkshopMedias { get; set; }
		public DbSet<WorkshopSession> WorkshopSessions { get; set; }
		public DbSet<WorkshopTicketType> WorkshopTicketTypes { get; set; }
		public DbSet<WorkshopType> WorkshopTypes { get; set; }
		public DbSet<UserOrganizationFollow> UserOrganizationFollows { get; set; }


		private readonly string _currentUser;

		public WokiwokiDbContext()
		{
		 
		}

		public WokiwokiDbContext(DbContextOptions<WokiwokiDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
		{
			_currentUser = httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";
		}


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder); 

			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				entityType.SetTableName(entityType.GetTableName()?.ToLower());

				foreach (var property in entityType.GetProperties())
				{
					var columnName = property.GetColumnName(StoreObjectIdentifier.Table(entityType.GetTableName(), entityType.GetSchema()));
					property.SetColumnName(columnName.ToLower());

					if (property.ClrType == typeof(Guid) || property.ClrType == typeof(Guid?))
					{
						property.SetColumnType("uuid");
					}
				}

				foreach (var key in entityType.GetKeys())
				{
					key.SetName(key.GetName().ToLower());
				}

				foreach (var fk in entityType.GetForeignKeys())
				{
					fk.SetConstraintName(fk.GetConstraintName().ToLower());
				}

				foreach (var index in entityType.GetIndexes())
				{
					index.SetDatabaseName(index.GetDatabaseName().ToLower());
				}
			}

			// AUDIT LOG
			modelBuilder.Entity<AuditLog>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Action).HasMaxLength(50);
				entity.Property(e => e.EntityName).HasMaxLength(100);
				entity.Property(e => e.OriginalValue).HasColumnType("text");
				entity.Property(e => e.NewValue).HasColumnType("text");

				entity.HasIndex(e => new { e.EntityName, e.Created }).HasDatabaseName("IX_AuditLog_Entity_Created");
				entity.HasIndex(e => e.LastModifiedBy).HasDatabaseName("IX_AuditLog_ModifiedBy");
			});

			// BOOKING
			modelBuilder.Entity<Booking>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.TotalPrice).HasColumnType("numeric(18,2)");  
				entity.HasOne(e => e.Workshop)
					.WithMany()
					.HasForeignKey(e => e.WorkshopId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasIndex(e => e.UserId).HasDatabaseName("IX_Booking_UserId");
				entity.HasIndex(e => e.WorkshopId).HasDatabaseName("IX_Booking_WorkshopId");
			});

			// ORGANIZATION
			modelBuilder.Entity<Organization>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
				entity.Property(e => e.Description).HasColumnType("text");
				entity.Property(e => e.LogoUrl).HasMaxLength(500);
				entity.Property(e => e.ContactEmail).HasMaxLength(255);
				entity.Property(e => e.ContactPhone).HasMaxLength(20);
				entity.Property(e => e.Street).HasMaxLength(255);
				entity.Property(e => e.Ward).HasMaxLength(100);
				entity.Property(e => e.District).HasMaxLength(100);
				entity.Property(e => e.Province).HasMaxLength(100);
			});

			// ORGANIZATION MEMBER
			modelBuilder.Entity<OrganizationMember>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Role).HasMaxLength(100).IsRequired();

				entity.HasOne(e => e.Organization)
					.WithMany(e => e.OrganizationMembers)
					.HasForeignKey(e => e.OrganizationId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			// REFRESH TOKEN
			modelBuilder.Entity<RefreshToken>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Token).HasMaxLength(500).IsRequired();
				entity.Ignore(e => e.IsActive); // Computed property

				entity.HasIndex(e => e.UserId).HasDatabaseName("IX_RefreshToken_UserId");
				entity.HasIndex(e => e.Token).HasDatabaseName("IX_RefreshToken_Token");
			});

			// TAG
			modelBuilder.Entity<Tag>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).HasMaxLength(100);
				entity.Property(e => e.Description).HasMaxLength(500);
				entity.Property(e => e.IconUrl).HasMaxLength(500);
			});

			// TICKET
			modelBuilder.Entity<Ticket>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Price).HasColumnType("numeric(18,2)");
				entity.Property(e => e.QrCodeImage).IsRequired(); 

				entity.HasOne(e => e.TicketType)
					.WithMany()
					.HasForeignKey(e => e.TicketTypeId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Booking)
					.WithMany()
					.HasForeignKey(e => e.BookingId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			// USER TAG PREFERENCE
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

			// USER WORKSHOP LIKE
			modelBuilder.Entity<UserWorkshopLike>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.HasIndex(e => new { e.UserId, e.WorkshopId }).IsUnique();

				entity.HasOne(e => e.Workshop)
					.WithMany(e=>e.Likes)
					.HasForeignKey(e => e.WorkshopId)
					.OnDelete(DeleteBehavior.Cascade);
			});


			// USER ORGANIZATION FOLLOW
			modelBuilder.Entity<UserOrganizationFollow>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.HasIndex(e => new { e.UserId, e.OrganizationId }).IsUnique();

				entity.HasOne(e => e.Organization)
					.WithMany(e => e.Followers)
					.HasForeignKey(e => e.OrganizationId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			// WORKSHOP
			modelBuilder.Entity<Workshop>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Title).HasMaxLength(255).IsRequired();
				entity.Property(e => e.ShortDescription).HasMaxLength(500);
				entity.Property(e => e.Description).HasColumnType("text").IsRequired();
				entity.Property(e => e.ImageUrl).HasMaxLength(500); 

				entity.HasOne(e => e.Organization)
					.WithMany()
					.HasForeignKey(e => e.OrganizationId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Category)
					.WithMany(e => e.Workshops)
					.HasForeignKey(e => e.CategoryId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasIndex(e => e.OrganizationId).HasDatabaseName("IX_Workshop_OrganizationId");
				entity.HasIndex(e => e.StartTime).HasDatabaseName("IX_Workshop_StartTime");
				entity.HasIndex(e => e.CategoryId).HasDatabaseName("IX_Workshop_CategoryId");
			});

			// WORKSHOP CATEGORY
			modelBuilder.Entity<Category>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
				entity.Property(e => e.Description).HasMaxLength(500);
				entity.Property(e => e.IconUrl).HasMaxLength(500);
				entity.Property(e => e.ImageUrl).HasMaxLength(500);
			});

			// WORKSHOP HERO MEDIA
			modelBuilder.Entity<WorkshopHeroMedia>(entity =>
			{
				entity.HasKey(e => e.Id);

				entity.HasOne(e => e.Workshop)
					.WithMany(e => e.WorkshopHeroMedias)
					.HasForeignKey(e => e.WorkshopId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(e => e.Gallery)
					.WithMany()
					.HasForeignKey(e => e.GalleryId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			// WORKSHOP MEDIA 
			modelBuilder.Entity<WorkshopMedia>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.ImageUrl).HasMaxLength(500);

				entity.HasOne(e => e.Workshop)
					.WithMany(e => e.WorkshopMedias)
					.HasForeignKey(e => e.WorkshopId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			// WORKSHOP SESSION
			modelBuilder.Entity<WorkshopSession>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Title).HasMaxLength(255).IsRequired();
				entity.Property(e => e.Description).HasColumnType("text").IsRequired();
				entity.Property(e => e.Location).HasMaxLength(255); 

				entity.HasOne(e => e.Workshop)
					.WithMany(e => e.WorkshopSessions)
					.HasForeignKey(e => e.WorkshopId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasIndex(e => e.WorkshopId).HasDatabaseName("IX_WorkshopSession_WorkshopId");
			});

			// WORKSHOP TICKET TYPE
			modelBuilder.Entity<WorkshopTicketType>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
				entity.Property(e => e.Description).HasMaxLength(500);
				entity.Property(e => e.Price)
					.HasColumnType("numeric(18,2)")
					.HasColumnName("price");
				entity.ToTable(t =>
				{
					t.HasCheckConstraint("CK_WorkshopTicketType_Price", "price >= 0");
				});
				entity.HasOne(e => e.WorkshopSession)
					.WithMany(e => e.WorkshopTicketTypes)
					.HasForeignKey(e => e.WorkshopSessionId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasIndex(e => e.WorkshopSessionId).HasDatabaseName("IX_WorkshopTicketType_WorkshopSessionId");
			});

			// WORKSHOP TYPE
			modelBuilder.Entity<WorkshopType>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
				entity.Property(e => e.Description).HasMaxLength(500);
				entity.Property(e => e.IconUrl).HasMaxLength(500);
			});

			// TAG - CATEGORY (many-to-many)
			modelBuilder.Entity<Tag>()
				.HasMany(e => e.Categories)
				.WithMany(e => e.Tags)
				.UsingEntity("category_tag");

			// TAG - WORKSHOP (many-to-many)
			modelBuilder.Entity<Tag>()
				.HasMany(e => e.Workshops)
				.WithMany(e => e.Tags)
				.UsingEntity("workshop_tag");

		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var modifiedEntries = ChangeTracker.Entries()
				.Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
				.ToList();

			var auditLogs = new List<AuditLog>();

			foreach (var entry in modifiedEntries)
			{
				var audit = new AuditLog
				{
					EntityName = entry.Entity.GetType().Name,
					LastModifiedBy = _currentUser,
					Created = DateTime.UtcNow,
					LastModified= DateTime.UtcNow,
					Action = entry.State.ToString()
				};

				if (entry.State == EntityState.Modified)
				{
					var original = new Dictionary<string, object>();
					var current = new Dictionary<string, object>();

					foreach (var prop in entry.OriginalValues.Properties)
					{
						var originalValue = entry.OriginalValues[prop]?.ToString();
						var currentValue = entry.CurrentValues[prop]?.ToString();

						if (originalValue != currentValue)
						{
							original[prop.Name] = originalValue;
							current[prop.Name] = currentValue;
						}
					}

					audit.OriginalValue = JsonSerializer.Serialize(original);
					audit.NewValue = JsonSerializer.Serialize(current);
				}
				else if (entry.State == EntityState.Added)
				{
					// chỉ log các field quan trọng thay vì full entity
					var values = entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p]?.ToString());
					audit.NewValue = JsonSerializer.Serialize(values);
				}
				else if (entry.State == EntityState.Deleted)
				{
					var values = entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p]?.ToString());
					audit.OriginalValue = JsonSerializer.Serialize(values);
				}

				auditLogs.Add(audit);
			}
			 
			var result = await base.SaveChangesAsync(cancellationToken);
			 
			if (auditLogs.Any())
			{
				AuditLogs.AddRange(auditLogs);
				await base.SaveChangesAsync(cancellationToken);
			}

			return result;
		}

	}

}
