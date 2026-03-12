using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Route.Query;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;
using Newtonsoft.Json.Linq;

namespace ERP.Mediator.Mediator.Route.Handler
{
    public class GetAllRouteHandler : IRequestHandler<GetAllRouteQuery, Tuple<IEnumerable<GetRoute>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        private readonly IAuthService authService;

        public GetAllRouteHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider, IAuthService authService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
            this.authService = authService;
        }

        public async Task<Tuple<IEnumerable<GetRoute>, long>> Handle(GetAllRouteQuery request, CancellationToken cancellationToken)
        {
            string Role = authService.GetCurrentUserRole();
            Expression<Func<Entities.Models.Route, bool>> predicate;
            if (Role == "ASE")
            {
                predicate = x => x.IsActive == true && x.IsDelete == false && x.TerritoryId == sessionProvider.Session.TerritoryId
                && (request.RegionId == 0 || x.Territory.Area.Zone.RegionId == request.RegionId)
                && (request.ZoneId == 0 || x.Territory.Area.ZoneId == request.ZoneId)
                && (request.AreaId == 0 || x.Territory.AreaId == request.AreaId)
                && x.Id != 23
                ;
            }
            else
            {
                predicate = x => x.IsActive == true
                && (request.RegionId == 0 || x.Territory.Area.Zone.RegionId == request.RegionId)
                && (request.ZoneId == 0 || x.Territory.Area.ZoneId == request.ZoneId)
                && (request.AreaId == 0 || x.Territory.AreaId == request.AreaId)
                && (request.TerritoryId == 0 || x.TerritoryId == request.TerritoryId)
                && x.Id != 23
                ;
            }

            Expression<Func<Entities.Models.Route, object>>[] includes = {
                //x => x.Territory,
                //x => x.RouteShop,
                //x => x.Territory.Area,
                //x => x.Territory.Area.Zone,
                //x => x.Territory.Area.Zone.Region,
                //x => x.Territory.Shop,
                //x => x.ShopRouteFrequency
            };

            List<string> thenInclude = new List<string>();
            thenInclude.Add("Territory.Shop.ShopRouteFrequency");
            //thenInclude.Add("RouteShop.Shop");
            //thenInclude.Add("RouteShop.Shop.Territory");
            //thenInclude.Add("RouteShop.Shop.Territory.Zone");

            Expression<Func<Entities.Models.Route, object>> OrderBy = null;
            Expression<Func<Entities.Models.Route, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.Route>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenInclude, includes);
            var route = mapper.Map<IEnumerable<GetRoute>>(entity.Item1.ToList()).ToList();
            //foreach (var item in route)
            //{
            //    var shopsToRemove = new List<GetShop>();
            //    var shoplist = item.Territory.Shop;
            //    foreach (var tershop in shoplist)
            //    {
            //        var AlreadySaveInOtherRoute = tershop.ShopRouteFrequency.Any(x => x.RouteId != item.Id && x.IsActive == true);
            //        if (AlreadySaveInOtherRoute == true)
            //        {

            //            shopsToRemove.Add(tershop);
            //        }
            //    }
            //    foreach (var shop in shopsToRemove)
            //    {
            //        item.Territory.Shop.Remove(shop);
            //    }
            //}
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

            return new Tuple<IEnumerable<GetRoute>, long>(route, entity.Item2);
        }
    }
}
