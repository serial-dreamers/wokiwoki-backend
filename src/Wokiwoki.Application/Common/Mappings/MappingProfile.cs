using AutoMapper;
using Wokiwoki.Application.DTOs;
using Wokiwoki.Application.DTOs.Response;
using Wokiwoki.Application.Features.Categories.Commands.CreateCategory;
using Wokiwoki.Application.Features.Categories.Commands.UpdateCategory;
using Wokiwoki.Application.Features.Workshops.Commands.CreateWorkshop;
using Wokiwoki.Domain.Entities;

namespace Wokiwoki.Application.Common.Mappings
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			// Workshop
			CreateMap<CreateWorkshopCommand, Workshop>();
			CreateMap<Workshop, SearchWorkshopDto>();
			CreateMap<Workshop, WorkshopDto>();

			//Category
			CreateMap<Category, CategoryDto>();
			CreateMap<CreateCategoryCommand, Category>();
			CreateMap<UpdateCategoryCommand, Category>();

			//Tag 
			CreateMap<Tag, TagDto>();

			//Review
			CreateMap<Review, ReviewDto>();

			//WorlkshopType
			CreateMap<WorkshopType, WorkshopTypeDto>();

			//WorkshopMedia
			CreateMap<WorkshopMedia, WorkshopMediaDto>();

			//WorkshopHeroMedia
			CreateMap<WorkshopHeroMedia, WorkshopHeroMediaDto>();
		}
	}
}
