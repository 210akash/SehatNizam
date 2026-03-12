using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.ShopOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShopOrder.Handler
{
    public class GetShopOrderByDealershipHandler : IRequestHandler<GetShopOrderByDealershipQuery, Tuple<IEnumerable<GetShopOrder>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetShopOrderByDealershipHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetShopOrder>, long>> Handle(GetShopOrderByDealershipQuery request, CancellationToken cancellationToken)
        {
            if (request.DealershipId == 0)
            {
                List<GetShopOrder> empty = new List<GetShopOrder>();
                return new Tuple<IEnumerable<GetShopOrder>, long>(empty, 0);
            }

            Expression<Func<Entities.Models.ShopOrder, bool>> predicate = null;
            var territoryId = unitOfWork.Repository<Entities.Models.Dealership>().GetFirstAsNoTrackingAsync(x => x.Id == request.DealershipId).Result.TerritoryId;

            if (request.ShopId == 0 && request.ShopOrderId == 0)
            {
                predicate = x => x.IsActive == true
                        && (request.StatusId == 0 || x.ShopOrderStatusId == request.StatusId)
                        && x.CreatedDate >= request.FDate
                        && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)
                        && x.Shop.TerritoryId == territoryId;
            }
            else if (request.ShopOrderId != 0 && request.ShopId != 0)
            {
                predicate = x => x.IsActive == true
                        && (request.StatusId == 0 || x.ShopOrderStatusId == request.StatusId)
                        && x.CreatedDate >= request.FDate
                        && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)
                        && x.Shop.TerritoryId == territoryId
                        && x.ShopId == request.ShopId
                        && x.Id == request.ShopOrderId;
            }
            else if (request.ShopId != 0 && request.ShopOrderId == 0)
            {
                predicate = x => x.IsActive == true
                        && (request.StatusId == 0 || x.ShopOrderStatusId == request.StatusId)
                        && x.CreatedDate >= request.FDate
                        && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)
                        && x.Shop.TerritoryId == territoryId
                        && x.ShopId == request.ShopId;
            }
            else if (request.ShopOrderId != 0 && request.ShopId == 0)
            {
                predicate = x => x.IsActive == true
                        && (request.StatusId == 0 || x.ShopOrderStatusId == request.StatusId)
                        && x.CreatedDate >= request.FDate
                        && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)
                        && x.Shop.TerritoryId == territoryId
                        && x.Id == request.ShopOrderId;
            }

            Expression<Func<Entities.Models.ShopOrder, object>>[] includes = {
                x => x.ShopOrderItems.Where(x => x.IsActive),
                x => x.Shop,
                x => x.CreatedBy,
                x => x.ShopOrderStatus,
            };

            Expression<Func<Entities.Models.ShopOrder, object>> OrderBy = null;
            Expression<Func<Entities.Models.ShopOrder, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            List<string> thenInclude = new()
            {
                "ShopOrderItems.Item",
                "ShopOrderItems.ShopDispatchDetails"
            };

            var entity = unitOfWork.Repository<Entities.Models.ShopOrder>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenInclude, includes);
            var shopOrder = mapper.Map<IEnumerable<GetShopOrder>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetShopOrder>, long>(shopOrder, entity.Item2);
        }
    }
}
