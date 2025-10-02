//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Wokiwoki.Application.Common.Models;
//using Wokiwoki.Domain.Entities;

//namespace Wokiwoki.Application.Features.Workshops.Queries.SearchWorkshop
//{
//    public record SearchWorkshopQuery (
//        //pascal con cac
//        Guid? cateId,
//        List<Guid>? tagIdList,
//        string? typeDate,
//        DateTime? startDate,
//        DateTime? endDate,
//        bool? isFree,
//        Guid? typeId
//        ) : IRequest<PaginatedList<SearchWorkshopDTO>>;
//    public class SearchWorkshopQueryHandler : IRequestHandler<SearchWorkshopQuery,SearchWorkshopDTO> 
//    {
//        public SearchWorkshopQueryHandler()
//        {
            
//        }
//        public Task<SearchWorkshopDTO> Handle(SearchWorkshopQuery request, CancellationToken cancellationToken)
//        {
//            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
//            DateTime vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

//            throw new NotImplementedException();
//        } 

//    }
//}
