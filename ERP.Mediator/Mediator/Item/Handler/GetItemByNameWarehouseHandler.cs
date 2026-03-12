using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Item.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Item.Handler
{
    public class GetItemByNameWarehouseHandler : IRequestHandler<GetItemByNameWarehouseQuery, GetStock>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetItemByNameWarehouseHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<GetStock> Handle(GetItemByNameWarehouseQuery request, CancellationToken cancellationToken)
        {
            request.ProjectId = sessionProvider.Session.SelectedWarehouseId != 0 ? sessionProvider.Session.SelectedWarehouseId :
                request.ProjectId;
            // Goods Receive Note
            var weightedAvgRatesGrn = (
                from grn in unitOfWork.Repository<Entities.Models.GRN>().GetAll().AsQueryable()
                join grnd in unitOfWork.Repository<GRNDetail>().GetAll().AsQueryable()
                    on grn.Id equals grnd.GRNId
                join ind in unitOfWork.Repository<InspectionDetail>().GetAll().AsQueryable()
                    on grnd.InspectionDetailId equals ind.Id
                join igpd in unitOfWork.Repository<IGPDetails>().GetAll().AsQueryable()
                    on ind.IGPDetailId equals igpd.Id
                join pod in unitOfWork.Repository<PurchaseOrderDetail>().GetAll().AsQueryable()
                    on igpd.PurchaseOrderDetailId equals pod.Id
                join pdd in unitOfWork.Repository<PurchaseDemandDetail>().GetAll().AsQueryable()
                    on pod.PurchaseDemandDetailId equals pdd.Id
                join item in unitOfWork.Repository<Entities.Models.Item>().GetAll().AsQueryable()
                    on pdd.ItemId equals item.Id
                where grn.IsActive
                      && grnd.IsActive
                      && grn.StatusId == 3
                      && item.Name == request.ItemId
                      && pdd.ProjectId == request.ProjectId
                select new
                {
                    ItemId = item.Id,
                    Rate = pod.UnitRate,
                    Quantity = grnd.Received
                }
            )
            .AsEnumerable()
            .GroupBy(x => x.ItemId)
            .Select(g => new
            {
                ItemId = g.Key,
                PurchaseAvgRate = Math.Round(g.Sum(x => x.Rate * x.Quantity) / g.Sum(x => x.Quantity), 2),
                Quantity = g.Sum(x => x.Quantity)
            })
             .ToDictionary(x => x.ItemId, x => (x.PurchaseAvgRate, x.Quantity));

            // Goods Sale Material
            var weightedAvgRatesSaleMaterial = (
                  from smDetail in unitOfWork.Repository<SaleMaterialDetail>().GetAll().AsQueryable()
                  join sm in unitOfWork.Repository<Entities.Models.SaleMaterial>().GetAll().AsQueryable()
                      on smDetail.SaleMaterialId equals sm.Id
                  where sm.CompanyId == sessionProvider.Session.CompanyId
                        && sm.IsActive
                        && smDetail.IsActive
                        && smDetail.ItemId == request.ItemId
                        && smDetail.ProjectId == request.ProjectId
                        && sm.StatusId == 3
                  select new
                  {
                      ItemId = smDetail.ItemId,
                      UnitRate = smDetail.Rate,
                      Quantity = smDetail.Quantity
                  }
              )
              .AsEnumerable()
              .GroupBy(x => x.ItemId)
              .Select(g => new
              {
                  ItemId = g.Key,
                  IssueAvgRate = Math.Round(g.Sum(x => x.UnitRate * x.Quantity) / g.Sum(x => x.Quantity), 2),
                  Quantity = g.Sum(x => x.Quantity)
              })
               .ToDictionary(x => x.ItemId, x => (x.IssueAvgRate, x.Quantity));

            // issuance
            var weightedAvgRatesissuance = (
                  from smDetail in unitOfWork.Repository<IssuanceDetail>().GetAll().AsQueryable()
                  join sm in unitOfWork.Repository<Entities.Models.Issuance>().GetAll().AsQueryable()
                      on smDetail.IssuanceId equals sm.Id
                  where sm.IndentRequest.Department.CompanyId == sessionProvider.Session.CompanyId
                        && sm.IsActive
                        && sm.ProjectId == request.ProjectId
                        && smDetail.IsActive
                        && smDetail.IndentRequestDetail.ItemId == request.ItemId
                        && sm.StatusId == 3
                  select new
                  {
                      ItemId = smDetail.IndentRequestDetail.ItemId,
                      UnitRate = smDetail.Rate,
                      Quantity = smDetail.Quantity
                  }
              )
              .AsEnumerable()
              .GroupBy(x => x.ItemId)
              .Select(g => new
              {
                  ItemId = g.Key,
                  IssueAvgRate = Math.Round(g.Sum(x => x.UnitRate * x.Quantity) / g.Sum(x => x.Quantity), 2),
                  Quantity = g.Sum(x => x.Quantity)
              })
               .ToDictionary(x => x.ItemId, x => (x.IssueAvgRate, x.Quantity));



            // Purchase Return
            var weightedAvgRatesPR = (
                from prd in unitOfWork.Repository<PurchaseReturnDetail>().GetAll().AsQueryable()
                join pr in unitOfWork.Repository<Entities.Models.PurchaseReturn>().GetAll().AsQueryable()
                    on prd.PurchaseReturnId equals pr.Id
                join grnd in unitOfWork.Repository<GRNDetail>().GetAll().AsQueryable()
                    on prd.GRNDetailId equals grnd.Id
                join ind in unitOfWork.Repository<InspectionDetail>().GetAll().AsQueryable()
                    on grnd.InspectionDetailId equals ind.Id
                join igpd in unitOfWork.Repository<IGPDetails>().GetAll().AsQueryable()
                    on ind.IGPDetailId equals igpd.Id
                join pod in unitOfWork.Repository<PurchaseOrderDetail>().GetAll().AsQueryable()
                    on igpd.PurchaseOrderDetailId equals pod.Id
                join pdd in unitOfWork.Repository<PurchaseDemandDetail>().GetAll().AsQueryable()
                    on pod.PurchaseDemandDetailId equals pdd.Id
                join item in unitOfWork.Repository<Entities.Models.Item>().GetAll().AsQueryable()
                    on pdd.ItemId equals item.Id
                where pr.IsActive
                      && prd.IsActive
                      && pdd.ItemId == request.ItemId
                      && prd.ProjectId == request.ProjectId
                      && pr.StatusId == 3
                select new
                {
                    ItemId = item.Id,
                    Rate = pod.UnitRate,
                    Quantity = prd.Quantity
                }
            )
            .AsEnumerable()
            .GroupBy(x => x.ItemId)
            .Select(g => new
            {
                ItemId = g.Key,
                PurchaseAvgRate = Math.Round(g.Sum(x => x.Rate * x.Quantity) / g.Sum(x => x.Quantity), 2),
                Quantity = g.Sum(x => x.Quantity)
            }).ToDictionary(x => x.ItemId, x => (x.PurchaseAvgRate, x.Quantity));


            // Dispatch 
            var weightedAvgRatesDispatch = (
                from dd in unitOfWork.Repository<DispatchDetail>().GetAll().AsQueryable()
                join Ddo in unitOfWork.Repository<DispatchOrder>().GetAll().AsQueryable()
                    on dd.DispatchOrderId equals Ddo.Id
                join dispatch in unitOfWork.Repository<ERP.Entities.Models.Dispatch>().GetAll().AsQueryable()
                 on Ddo.DispatchId equals dispatch.Id
                join oi in unitOfWork.Repository<OrderItems>().GetAll().AsQueryable()
                    on dd.OrderItemId equals oi.Id
                where dispatch.IsActive
                      && Ddo.IsActive
                      && oi.ItemId == request.ItemId
                      && dd.ProjectId == request.ProjectId
                      && dispatch.StatusId == 3
                select new
                {
                    ItemId = oi.ItemId,
                    Rate = dd.CostSheet.CostPerPet,
                    Quantity = dd.Quantity
                }
            )
            .AsEnumerable()
            .GroupBy(x => x.ItemId)
            .Select(g => new
            {
                ItemId = g.Key,
                DispatchAvgRate = Math.Round(g.Sum(x => x.Rate * x.Quantity) / g.Sum(x => x.Quantity), 2),
                Quantity = g.Sum(x => x.Quantity)
            }).ToDictionary(x => x.ItemId, x => (x.DispatchAvgRate, x.Quantity));

            // Sale Return
            var weightedAvgRatesSR = (
                from srd in unitOfWork.Repository<SaleReturnDetail>().GetAll().AsQueryable()
                join sr in unitOfWork.Repository<Entities.Models.SaleReturn>().GetAll().AsQueryable()
                    on srd.SaleReturnId equals sr.Id
                join dd in unitOfWork.Repository<DispatchDetail>().GetAll().AsQueryable()
                    on srd.DispatchDetailId equals dd.Id
                join oi in unitOfWork.Repository<OrderItems>().GetAll().AsQueryable()
                    on dd.OrderItemId equals oi.Id
                join item in unitOfWork.Repository<Entities.Models.Item>().GetAll().AsQueryable()
                    on oi.ItemId equals item.Id
                where sr.IsActive
                      && srd.IsActive
                      && oi.ItemId == request.ItemId
                      && srd.ProjectId == request.ProjectId
                      && sr.StatusId == 3
                select new
                {
                    ItemId = item.Id,
                    Rate = dd.CostSheet.CostPerPet,
                    Quantity = srd.Quantity
                }
            )
            .AsEnumerable()
            .GroupBy(x => x.ItemId)
            .Select(g => new
            {
                ItemId = g.Key,
                SaleAvgRate = Math.Round(g.Sum(x => x.Rate * x.Quantity) / g.Sum(x => x.Quantity), 2),
                Quantity = g.Sum(x => x.Quantity)
            }).ToDictionary(x => x.ItemId, x => (x.SaleAvgRate, x.Quantity));

            // Assign LastPurchaseRate

            // Get rates and quantities
            (decimal purchaseRate, decimal grnQty) = weightedAvgRatesGrn.TryGetValue(request.ItemId, out var grnData) ? grnData : (0m, 0m);
            (decimal issuanceRate, decimal issuanceQty) = weightedAvgRatesissuance.TryGetValue(request.ItemId, out var issuanceData) ? issuanceData : (0m, 0m);
            (decimal issueRate, decimal saleQty) = weightedAvgRatesSaleMaterial.TryGetValue(request.ItemId, out var saleData) ? saleData : (0m, 0m);
            (decimal prRate, decimal prQty) = weightedAvgRatesPR.TryGetValue(request.ItemId, out var prData) ? prData : (0m, 0m);
            (decimal srRate, decimal srQty) = weightedAvgRatesSR.TryGetValue(request.ItemId, out var srData) ? srData : (0m, 0m);
            (decimal dispatchRate, decimal dispatchQty) = weightedAvgRatesDispatch.TryGetValue(request.ItemId, out var dispatchData) ? dispatchData : (0m, 0m);

            // Compute remaining stock and value
            decimal remainingQty = (grnQty + srQty) - (saleQty + prQty + dispatchQty + issuanceQty);
            decimal remainingValue = ((grnQty * purchaseRate) + (srQty * srRate)) - ((saleQty * issueRate) + (issuanceQty * issuanceRate) + (prQty * prRate) + (dispatchQty * dispatchRate));
            decimal weightedAvgRate = remainingQty > 0 ? Math.Round(remainingValue / remainingQty, 2) : 0;

            // Prepare result
            var _Item = new GetStock
            {
                Quantity = remainingQty,
                Rate = weightedAvgRate
            };

            return _Item;
        }
    }
}
