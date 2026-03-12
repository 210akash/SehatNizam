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
    public class GetShopOrderByUserHandler : IRequestHandler<GetShopOrderByUserQuery, Tuple<IEnumerable<GetShopOrder>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetShopOrderByUserHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetShopOrder>, long>> Handle(GetShopOrderByUserQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.ShopOrder, bool>> predicate = null;
            if (request.ShopId == 0 && request.ShopOrderId == 0)
            {
                predicate = x => x.IsActive == true
                        && (request.StatusId == 0 || x.ShopOrderStatusId == request.StatusId)
                        && x.CreatedDate >= request.FDate
                        && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)
                        && x.CreatedById == request.CreatedById;
            }
            else if (request.ShopOrderId != 0 && request.ShopId != 0)
            {
                predicate = x => x.IsActive == true
                        && (request.StatusId == 0 || x.ShopOrderStatusId == request.StatusId)
                        && x.CreatedDate >= request.FDate
                        && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)
                        && x.CreatedById == request.CreatedById
                        && x.ShopId == request.ShopId
                        && x.Id == request.ShopOrderId;
            }
            else if (request.ShopId != 0 && request.ShopOrderId == 0)
            {
                predicate = x => x.IsActive == true
                        && (request.StatusId == 0 || x.ShopOrderStatusId == request.StatusId)
                        && x.CreatedDate >= request.FDate
                        && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)
                        && x.CreatedById == request.CreatedById
                        && x.ShopId == request.ShopId;
            }
            else if (request.ShopOrderId != 0 && request.ShopId == 0)
            {
                predicate = x => x.IsActive == true
                        && (request.StatusId == 0 || x.ShopOrderStatusId == request.StatusId)
                        && x.CreatedDate >= request.FDate
                        && x.CreatedDate <= request.TDate.Value.AddDays(1).AddSeconds(-1)
                        && x.CreatedById == request.CreatedById
                        && x.Id == request.ShopOrderId;
            }

            Expression<Func<Entities.Models.ShopOrder, object>>[] includes = {
                x => x.ShopOrderItems.Where(x => x.IsActive),
                x => x.Shop,
                //x => x.Shop.Territory.Dealership,
                x => x.CreatedBy,
                x => x.ShopOrderStatus,
            };

            Expression<Func<Entities.Models.ShopOrder, object>> OrderBy = null;
            Expression<Func<Entities.Models.ShopOrder, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            List<string> thenInclude = new List<string>
            {
                "ShopOrderItems.Item"
            };

            var entity = unitOfWork.Repository<Entities.Models.ShopOrder>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenInclude, includes);
            var shopOrder = mapper.Map<IEnumerable<GetShopOrder>>(entity.Item1.ToList()).ToList();
            foreach (var item in shopOrder)
            {
                var Dealership = await unitOfWork.Repository<Entities.Models.Dealership>().GetFirstAsNoTrackingAsync(y=> y.IsActive == true && y.TerritoryId == item.Shop.TerritoryId);
                item.DealershipId = Dealership != null ? Dealership.Id : 0;
            }
            return new Tuple<IEnumerable<GetShopOrder>, long>(shopOrder, entity.Item2);
        }
    }
}
