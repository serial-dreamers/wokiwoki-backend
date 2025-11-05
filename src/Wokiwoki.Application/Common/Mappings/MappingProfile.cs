using AutoMapper;
using Wokiwoki.Application.Common.Models;
using Wokiwoki.Application.DTOs;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Bookings.Commands;
using Wokiwoki.Application.Features.Categories.Commands.CreateCategory;
using Wokiwoki.Application.Features.Categories.Commands.UpdateCategory;
using Wokiwoki.Application.Features.Reviews.Command;
using Wokiwoki.Application.Features.ScheduleTickets.Command;
using Wokiwoki.Application.Features.Tags.Commands.CreateTag;
using Wokiwoki.Application.Features.Tags.Commands.UpdateTag;
using Wokiwoki.Application.Features.WorkshopHeroMedias.Queries.GetHeroMedias;
using Wokiwoki.Application.Features.Workshops.Commands.CreateWorkshop;
using Wokiwoki.Application.Features.WorkshopSchedules.Commands.CreateSchedule;
using Wokiwoki.Application.Features.WorkshopSchedules.Commands.UpdateSchedule;
using Wokiwoki.Application.Features.WorkshopSessions.Commands;
using Wokiwoki.Domain.Entities;
using Wokiwoki.Domain.Enums;

namespace Wokiwoki.Application.Common.Mappings
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
            // Workshop
            CreateMap<CreateWorkshopCommand, Workshop>()
    // Chỉ map những field trong CreateWorkshopCommand
    .ForMember(dest => dest.Description,
        opt => opt.PreCondition(src => !string.IsNullOrWhiteSpace(src.Description)))
    .ForMember(dest => dest.ImageUrl,
        opt => opt.PreCondition(src => !string.IsNullOrWhiteSpace(src.ImageUrl)))
    .ForMember(dest => dest.OnlineEventUrl,
        opt => opt.PreCondition(src => !string.IsNullOrWhiteSpace(src.OnlineEventUrl)))
    .ForMember(dest => dest.RefundPolicy,
        opt => opt.PreCondition(src => Enum.IsDefined(typeof(RefundPolicyType), src.RefundPolicy)))
    .ForMember(dest => dest.RefundPolicyDescription,
        opt => opt.PreCondition(src => !string.IsNullOrWhiteSpace(src.RefundPolicyDescription)))
    .ForMember(dest => dest.RegistrationDeadlineHours,
        opt => opt.PreCondition(src => src.RegistrationDeadlineHours.HasValue))

    // ⚠️ Bỏ qua toàn bộ các field đã tạo từ draft (không map)
    .ForMember(dest => dest.Title, opt => opt.Ignore())
    .ForMember(dest => dest.OrganizationId, opt => opt.Ignore())
    .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
    .ForMember(dest => dest.Tags, opt => opt.Ignore())
    .ForMember(dest => dest.StartingPrice, opt => opt.Ignore())
    .ForMember(dest => dest.DeliveryType, opt => opt.Ignore())
    .ForMember(dest => dest.ScheduleType, opt => opt.Ignore())
    .ForMember(dest => dest.Summary, opt => opt.Ignore())
    .ForMember(dest => dest.DurationMinutes, opt => opt.Ignore())
    .ForMember(dest => dest.DefaultCapacity, opt => opt.Ignore()); 



			CreateMap<Workshop, SearchWorkshopDto>();
			CreateMap<Workshop, WorkshopDto>();
			CreateMap<Workshop, CreatedDto>();


			//Category
			CreateMap<Category, CategoryDto>();
			CreateMap<CreateCategoryCommand, Category>();
			CreateMap<UpdateCategoryCommand, Category>();

			//Tag 
			CreateMap<Tag, TagDto>();
			CreateMap<CreateTagCommand, Tag>();
			CreateMap<UpdateTagCommand, Tag>(); 

			//Review
			CreateMap<Review, ReviewDto>(); 


			//WorkshopMedia
			CreateMap<WorkshopMedia, WorkshopMediaDto>(); 

			//WorkshopHeroMedia
			CreateMap<WorkshopHeroMedia, WorkshopHeroMediaDto>();

			CreateMap<WorkshopHeroMedia, WorkshopHeroMediaDtoWithWsId>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.MediaId, opt => opt.MapFrom(src => src.WorkshopMedia.Id))
			.ForMember(dest => dest.HeroType, opt => opt.MapFrom(src => src.HeroType))
			.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.WorkshopMedia.ImageUrl))
			.ForMember(dest => dest.WorkshopId, opt => opt.MapFrom(src => src.WorkshopId))
			.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

			//Organization
			CreateMap<Organization, OrganizationDto>();

            //WorkshopSchedule
            CreateMap<CreateScheduleCommand, WorkshopSchedule>()
    .ForMember(dest => dest.StartTime,
               opt => opt.MapFrom(src => TimeOnly.Parse(src.StartTime)))
    .ForMember(dest => dest.EndTime,
               opt => opt.MapFrom(src => TimeOnly.Parse(src.EndTime)));
            CreateMap<WorkshopSchedule, WorkshopScheduleDto>();
            CreateMap<UpdateScheduleCommand, WorkshopSchedule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WorkshopId, opt => opt.Ignore());

            //WorkshopSession
            CreateMap<CreateSessionCommand,  WorkshopSession>();
            CreateMap<UpdateSessionCommand, WorkshopSession>();
			CreateMap<WorkshopSession, WorkshopSessionDto>();

			CreateMap<Create1MonthSessionCommand, WorkshopSession>()
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.ScheduleId, opt => opt.Ignore());

			CreateMap<WorkshopSession, CreatedDto>(); 

			//Booking
			CreateMap<CreateBookingCommand, Booking>()
            .ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets));
			

            //Ticket
            CreateMap<TicketCreateDTO, Ticket>();

            //ScheduleTicket
            CreateMap<CreateScheduleTicketCommand, WorkshopScheduleTicket>();
			CreateMap<WorkshopScheduleTicket, WorkshopScheduleTicketDto>();

			//Review
			CreateMap<CreateReviewCommand, Review>();

			//Discover Workshop
			CreateMap<Workshop, DiscoverWorkshopDto>();
			CreateMap<Organization, DiscoverOrganizationDto>();

		}
	}
}
