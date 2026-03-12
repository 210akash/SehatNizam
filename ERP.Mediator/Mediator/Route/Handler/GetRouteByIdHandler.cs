using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Route.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Route.Handler
{
    public class GetRouteByIdHandler : IRequestHandler<GetRouteByIdQuery, GetRoute>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetRouteByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetRoute> Handle(GetRouteByIdQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Route, bool>> predicate;
            predicate = x => x.IsActive == true && x.Id == request.Id
                ;

            Expression<Func<Entities.Models.Route, object>>[] includes = {
                x => x.Territory,
                x => x.RouteShop,
                x => x.Territory.Area,
                x => x.Territory.Area.Zone,
                x => x.Territory.Area.Zone.Region,
                x => x.Territory.Shop,
                x => x.ShopRouteFrequency
            };

            List<string> thenInclude = new List<string>();
            thenInclude.Add("Territory.Shop.ShopRouteFrequency");

            Expression<Func<Entities.Models.Route, object>> OrderBy = null;
            Expression<Func<Entities.Models.Route, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.Route>().GetPagingWhereAsNoTrackingAsync(predicate, null, OrderBy, OrderByDesc, thenInclude, includes);
            var route = mapper.Map<IEnumerable<GetRoute>>(entity.Item1.ToList()).ToList();

            foreach (var item in route)
            {
                var shoplist = item.Territory.Shop.ToList(); // Create a copy to avoid modifying the collection during iteration

                // Remove shops that meet the condition in one pass
                foreach (var tershop in shoplist)
                {
                    var AlreadySaveInOtherRoute = tershop.ShopRouteFrequency.Any(x => x.RouteId != item.Id && x.IsActive == true);
                    if (AlreadySaveInOtherRoute)
                    {
                        item.Territory.Shop.Remove(tershop); // Remove directly from the original collection
                    }
                }
            }

            return route.FirstOrDefault();
        }
    }
}
