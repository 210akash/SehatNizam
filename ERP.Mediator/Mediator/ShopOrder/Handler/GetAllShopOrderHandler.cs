using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ShopOrder.Query;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Implementation;
using ERP.Services.Interfaces;
using MediatR;

namespace ERP.Mediator.Mediator.ShopOrder.Handler
{
    public class GetAllShopOrderHandler : IRequestHandler<GetAllShopOrderQuery, Tuple<IEnumerable<GetShopOrder>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuthService authService;
        private readonly SessionProvider sessionProvider;

        public GetAllShopOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.authService = authService;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetShopOrder>, long>> Handle(GetAllShopOrderQuery request, CancellationToken cancellationToken)
        {
            string role = authService.GetCurrentUserRole();
            Expression<Func<Entities.Models.Order, bool>> predicate = null;
            if (role == "Distributor" || role == "ASE" || role == "DSF")
            {
                predicate = x => x.IsActive == true
                    && (request.StatusId == 0 || x.OrderStatusId == request.StatusId)
                    && x.CreatedDate >= request.FDate
                    && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)
                    && x.ShopId != 0 && x.ShopId != null && x.Shop.TerritoryId == sessionProvider.Session.TerritoryId

                    && (request.RegionId == 0 || x.Shop.Territory.Area.Zone.RegionId == request.RegionId)
                    && (request.ZoneId == 0 || x.Shop.Territory.Area.ZoneId == request.ZoneId)
                    && (request.AreaId == 0 || x.Shop.Territory.AreaId == request.AreaId)
                    && (request.TerritoryId == 0 || x.Shop.TerritoryId == request.TerritoryId)
                    && (request.ShopId == 0 || x.ShopId == request.ShopId)
                    ;
            }
            else
            {
                predicate = x => x.IsActive == true
                    && (request.StatusId == 0 || x.OrderStatusId == request.StatusId)
                    && x.CreatedDate >= request.FDate
                    && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)
                    && x.ShopId != 0 && x.ShopId != null

                    && (request.RegionId == 0 || x.Shop.Territory.Area.Zone.RegionId == request.RegionId)
                    && (request.ZoneId == 0 || x.Shop.Territory.Area.ZoneId == request.ZoneId)
                    && (request.AreaId == 0 || x.Shop.Territory.AreaId == request.AreaId)
                    && (request.TerritoryId == 0 || x.Shop.TerritoryId == request.TerritoryId)
                    && (request.ShopId == 0 || x.ShopId == request.ShopId)
                    ;
            }

            Expression<Func<Entities.Models.Order, object>>[] includes = {
                x => x.Shop,
                x => x.CreatedBy,
                x => x.Shop.Territory.Area.Zone,
                x => x.Shop.Territory,
                x => x.OrderStatus,
                x => x.OrderProcess,
            };

            Expression<Func<Entities.Models.Order, object>> OrderBy = null;
            Expression<Func<Entities.Models.Order, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            List<string> thenInclude = new List<string>();
            thenInclude.Add("OrderProcess.FromStatus");
            thenInclude.Add("OrderProcess.ToStatus");

            var entity = unitOfWork.Repository<Entities.Models.Order>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenInclude, includes);
            var shopOrder = mapper.Map<IEnumerable<GetShopOrder>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetShopOrder>, long>(shopOrder, entity.Item2);
        }
    }
}
