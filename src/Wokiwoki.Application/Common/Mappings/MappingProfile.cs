using AutoMapper;
using Wokiwoki.Application.Features.Categories.Queries.GetCategories;
using Wokiwoki.Application.Features.Tags.Queries;
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

			
			//Category
			CreateMap<Category, CategoryDto>();

			//Tag 
			CreateMap<Tag, TagDto>();

		}
	}
}
