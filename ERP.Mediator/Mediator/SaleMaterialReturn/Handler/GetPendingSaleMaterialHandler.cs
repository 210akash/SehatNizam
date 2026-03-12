using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.SaleMaterialReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.SaleMaterialReturn.Handler
{
    public class GetPendingSaleMaterialHandler : IRequestHandler<GetPendingSaleMaterialQuery, List<GetSaleMaterial>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetPendingSaleMaterialHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetSaleMaterial>> Handle(GetPendingSaleMaterialQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<ERP.Entities.Models.SaleMaterial, object>> orderByExpression = x => x.CreatedDate;
            var rawOrders = await unitOfWork.Repository<ERP.Entities.Models.SaleMaterial>().GetAsync(
                x => x.IsActive 
                && x.ProjectId == sessionProvider.Session.SelectedWarehouseId
                && x.StatusId == 3,
                includeProperties: "SaleMaterialDetail,Dealership",
                orderByDec: query => query.OrderByDescending(orderByExpression)
            );

            var searchParam = request.searchParam?.Trim();
            var PurchaseOrders = string.IsNullOrWhiteSpace(searchParam)
         ? rawOrders.Take(10).ToList()
         : rawOrders
             .Where(x =>
             {
                 var digitsOnly = new string(x.Code.Where(char.IsDigit).ToArray());
                 return digitsOnly.Contains(searchParam);
             })
             .Take(10)
             .ToList();

            var processedDetailIds = unitOfWork.Repository<SaleMaterialReturnDetail>()
                .GetAll().Where(x => x.IsActive)
                .GroupBy(pd => pd.SaleMaterialDetailId)
                .Select(g => new
                {
                    DetailId = g.Key,
                    TotalReceivedQty = g.Sum(pd => pd.Quantity)
                })
                .ToDictionary(x => x.DetailId, x => x.TotalReceivedQty);

            List<ERP.Entities.Models.SaleMaterial> pendingPurchaseOrder = new();

            foreach (var item in PurchaseOrders)
            {
                if (item.Id != request.SaleMaterialId)
                {
                    bool hasPendingDetail = false;

                    foreach (var detail in item.SaleMaterialDetail.Where(y => y.IsActive))
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

            return mapper.Map<List<GetSaleMaterial>>(pendingPurchaseOrder);
        }
    }
}
