using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.SaleReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.SaleReturn.Handler
{
    public class GetPendingDCHandler : IRequestHandler<GetPendingDCQuery, List<GetDispatchOrder>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetPendingDCHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetDispatchOrder>> Handle(GetPendingDCQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<DispatchOrder, object>> orderByExpression = x => x.CreatedDate;
            var rawOrders = await unitOfWork.Repository<DispatchOrder>().GetAsync(
                x => x.IsActive 
                && x.Dispatch.ProjectId == sessionProvider.Session.SelectedWarehouseId
                && x.StatusId == 50,
                includeProperties: "Order,Order.Dealership,Dispatch,DispatchDetail",
                orderByDec: query => query.OrderByDescending(orderByExpression)
            );

            var searchParam = request.searchParam?.Trim();
            var PurchaseOrders = string.IsNullOrWhiteSpace(searchParam)
         ? rawOrders.Take(10).ToList()
         : rawOrders
             .Where(x =>
             {
                 var digitsOnly = new string(x.DCCode.Where(char.IsDigit).ToArray());
                 return digitsOnly.Contains(searchParam);
             })
             .Take(10)
             .ToList();

            var processedDetailIds = unitOfWork.Repository<SaleReturnDetail>()
                .GetAll(null,null, "SaleReturn").Where(x => x.IsActive
                        && x.SaleReturn.ProjectId == sessionProvider.Session.SelectedWarehouseId
                )
                .GroupBy(pd => pd.DispatchDetailId)
                .Select(g => new
                {
                    DetailId = g.Key,
                    TotalReceivedQty = g.Sum(pd => pd.Quantity)
                })
                .ToDictionary(x => x.DetailId, x => x.TotalReceivedQty);

            List<DispatchOrder> pendingPurchaseOrder = new();

            foreach (var item in PurchaseOrders)
            {
                if (item.Id != request.DispatchedOrderId)
                {
                    bool hasPendingDetail = false;

                    foreach (var detail in item.DispatchDetail.Where(y => y.IsActive))
                    {
                        var totalOrderedQty = processedDetailIds.TryGetValue(detail.Id, out var OrderedQty) ? OrderedQty : 0;

                        if (detail.Quantity > totalOrderedQty)
                        {
                            hasPendingDetail = true;
                            break;
                        }
                    }

                    if (hasPendingDetail)
                    {
                        pendingPurchaseOrder.Add(item);
                    }
                }
                else
                    pendingPurchaseOrder.Add(item);
            }

            return mapper.Map<List<GetDispatchOrder>>(pendingPurchaseOrder);
        }
    }
}
