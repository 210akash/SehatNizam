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
using ERP.Mediator.Mediator.App.Query;
using ERP.Mediator.Mediator.ShopDispatch.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShopDispatch.Handler
{
    public class GetPendingShopOrderItemsForDispatchHandler : IRequestHandler<GetPendingShopOrderItemsForDispatchQuery, List<GetShopOrderItems>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        private readonly IMediator mediator;

        public GetPendingShopOrderItemsForDispatchHandler(IMediator mediator,IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
            this.mediator = mediator;
        }

        public async Task<List<GetShopOrderItems>> Handle(GetPendingShopOrderItemsForDispatchQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<ShopOrderItems, object>> orderByExpression = x => x.Id;
            Expression<Func<ShopOrderItems, object>>[] includes = {
                x => x.Item,
             };

            Expression<Func<ShopOrderItems, bool>> predicate = x => x.IsActive == true
                   && x.ShopOrderId == request.ShopOrderId;

            var entity = unitOfWork.Repository<ShopOrderItems>().GetPagingWhereAsNoTrackingAsync(predicate, null, orderByExpression, null, null, includes);
            var ShopOrderItems1 = entity.Item1.ToList();
            var ShopOrderItems = mapper.Map<IEnumerable<GetShopOrderItems>>(ShopOrderItems1).ToList();

            var processedDetailIds = unitOfWork.Repository<ShopDispatchDetail>()
                .FindAll(x => x.IsActive == true && x.ShopOrderItem.ShopOrderId == request.ShopOrderId && x.ShopDispatch.IsActive == true)
                .GroupBy(pd => pd.ShopOrderItemId)
                .Select(g => new { DetailId = g.Key, TotalOrderedQty = g.Sum(pd => pd.Quantity) })
                .ToDictionary(x => x.DetailId, x => x.TotalOrderedQty);


            GetDealershipStockBalanceQuery getDealershipStockBalanceQuery = new()
            {
                DealershipId = request.DealershipId,
                AppDateTime = DateTime.Now
            };

            var data = await this.mediator.Send(getDealershipStockBalanceQuery);

            List<GetShopOrderItems> pendingShopOrders = new();

            if (request.ShopDispatchId == 0)
            {
                pendingShopOrders = ShopOrderItems
                   .Where(detail =>
                   {
                       var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;
                       return detail.Quantity > totalOrderedQty;
                   })
                   .ToList();

                foreach (var detail in pendingShopOrders)
                {
                    var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;
                    detail.Item.StockQty = data.FirstOrDefault(y => y.Id == detail.ItemId)?.Balance ?? 0m;
                    detail.Quantity -= Convert.ToInt32(totalOrderedQty);
                }
            }
            else
            {
                pendingShopOrders = ShopOrderItems;
            }

            var PurchaseDemandDetail = mapper.Map<IEnumerable<GetShopOrderItems>>(pendingShopOrders).ToList();
            return PurchaseDemandDetail;
        }
    }
}
