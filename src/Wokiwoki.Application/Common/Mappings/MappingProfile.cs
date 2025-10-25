using AutoMapper; 
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Categories.Commands.CreateCategory;
using Wokiwoki.Application.Features.Categories.Commands.UpdateCategory;
using Wokiwoki.Application.Features.Tags.Commands.CreateTag;
using Wokiwoki.Application.Features.Tags.Commands.UpdateTag;
using Wokiwoki.Application.Features.Workshops.Commands.CreateWorkshop;
using Wokiwoki.Application.Features.WorkshopSchedules.Commands.CreateSchedule;
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

			//Organization
			CreateMap<Organization, OrganizationDto>();

            //WorkshopSchedule
            CreateMap<CreateScheduleCommand, WorkshopSchedule>();

            //WorkshopSession
            CreateMap<CreateSessionCommand,  WorkshopSession>();
        }
	}
}
