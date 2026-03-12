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
using Stripe;

namespace ERP.Mediator.Mediator.Item.Handler
{
    public class GetStockByWarehouseHandler : IRequestHandler<GetSockByWarehouseQuery, GetStock>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetStockByWarehouseHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<GetStock> Handle(GetSockByWarehouseQuery request, CancellationToken cancellationToken)
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
                      && pdd.ItemId == request.ItemId
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
                        && smDetail.Quantity > 0 
                        && smDetail.ItemId == request.ItemId
                        && sm.ProjectId == request.ProjectId
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
                        && smDetail.Quantity > 0
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
                      && prd.Quantity > 0
                      && pdd.ItemId == request.ItemId
                      && pr.ProjectId == request.ProjectId
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
                      && dd.Quantity > 0
                      && oi.ItemId == request.ItemId
                      && dispatch.ProjectId == request.ProjectId
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
                      && srd.Quantity > 0
                      && oi.ItemId == request.ItemId
                      && sr.ProjectId == request.ProjectId
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


            // Warehouse Transfer (Out)
            var weightedAvgRatesWarehouseTransferOut = (
                  from smDetail in unitOfWork.Repository<WarehouseTransferDetail>().GetAll().AsQueryable()
                  join sm in unitOfWork.Repository<Entities.Models.WarehouseTransfer>().GetAll().AsQueryable()
                      on smDetail.WarehouseTransferId equals sm.Id
                  where sm.CompanyId == sessionProvider.Session.CompanyId
                        && sm.IsActive
                        && smDetail.IsActive
                        && smDetail.Quantity > 0
                        && smDetail.ItemId == request.ItemId
                        && sm.TransferFromId == request.ProjectId
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
                  TransferAvgRate = Math.Round(g.Sum(x => x.UnitRate * x.Quantity) / g.Sum(x => x.Quantity), 2),
                  Quantity = g.Sum(x => x.Quantity)
              })
               .ToDictionary(x => x.ItemId, x => (x.TransferAvgRate, x.Quantity));

            // Warehouse Transfer (In)
            var weightedAvgRatesWarehouseTransferIn = (
                  from smDetail in unitOfWork.Repository<WarehouseTransferDetail>().GetAll().AsQueryable()
                  join sm in unitOfWork.Repository<Entities.Models.WarehouseTransfer>().GetAll().AsQueryable()
                      on smDetail.WarehouseTransferId equals sm.Id
                  where sm.CompanyId == sessionProvider.Session.CompanyId
                        && sm.IsActive
                        && smDetail.IsActive
                        && smDetail.Quantity > 0
                        && smDetail.ItemId == request.ItemId
                        && sm.TransferToId == request.ProjectId
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
                  TransferAvgRate = Math.Round(g.Sum(x => x.UnitRate * x.Quantity) / g.Sum(x => x.Quantity), 2),
                  Quantity = g.Sum(x => x.Quantity)
              })
               .ToDictionary(x => x.ItemId, x => (x.TransferAvgRate, x.Quantity));


            // Goods Sale Material Return
            var weightedAvgRatesSaleMaterialReturn = (
                  from smrDetail in unitOfWork.Repository<SaleMaterialReturnDetail>().GetAll().AsQueryable()
                  join smr in unitOfWork.Repository<Entities.Models.SaleMaterialReturn>().GetAll().AsQueryable()
                      on smrDetail.SaleMaterialReturnId equals smr.Id
                  join sm in unitOfWork.Repository<Entities.Models.SaleMaterial>().GetAll().AsQueryable()
                      on smr.SaleMaterialId equals sm.Id
                  join smDetail in unitOfWork.Repository<Entities.Models.SaleMaterialDetail>().GetAll().AsQueryable()
                      on sm.Id equals smDetail.SaleMaterialId
                  where smr.SaleMaterial.CompanyId == sessionProvider.Session.CompanyId
                        && smr.IsActive
                        && smrDetail.IsActive
                        && smrDetail.Quantity > 0
                        && smrDetail.SaleMaterialDetail.ItemId == request.ItemId
                        && smr.ProjectId == request.ProjectId
                        && smr.StatusId == 3
                  select new
                  {
                      ItemId = smrDetail.SaleMaterialDetail.ItemId,
                      UnitRate = smrDetail.SaleMaterialDetail.Rate,
                      Quantity = smrDetail.Quantity
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

            // Assign LastPurchaseRate

            // Get rates and quantities
            (decimal purchaseRate, decimal grnQty) = weightedAvgRatesGrn.TryGetValue(request.ItemId, out var grnData) ? grnData : (0m, 0m);
            (decimal issuanceRate, decimal issuanceQty) = weightedAvgRatesissuance.TryGetValue(request.ItemId, out var issuanceData) ? issuanceData : (0m, 0m);
            (decimal issueRate, decimal saleQty) = weightedAvgRatesSaleMaterial.TryGetValue(request.ItemId, out var saleData) ? saleData : (0m, 0m);
            (decimal prRate, decimal prQty) = weightedAvgRatesPR.TryGetValue(request.ItemId, out var prData) ? prData : (0m, 0m);
            (decimal srRate, decimal srQty) = weightedAvgRatesSR.TryGetValue(request.ItemId, out var srData) ? srData : (0m, 0m);
            (decimal dispatchRate, decimal dispatchQty) = weightedAvgRatesDispatch.TryGetValue(request.ItemId, out var dispatchData) ? dispatchData : (0m, 0m);
            (decimal transferOutRate, decimal transferOutQty) = weightedAvgRatesWarehouseTransferOut.TryGetValue(request.ItemId, out var TransferOutData) ? TransferOutData : (0m, 0m);
            (decimal transferInRate, decimal transferInQty) = weightedAvgRatesWarehouseTransferIn.TryGetValue(request.ItemId, out var TransferInData) ? TransferInData : (0m, 0m);
            (decimal saleMaterialReturnRate, decimal saleMaterialReturnQty) = weightedAvgRatesSaleMaterialReturn.TryGetValue(request.ItemId, out var SaleMaterialReturnData) ? SaleMaterialReturnData : (0m, 0m);

            // Compute remaining stock and value
            decimal remainingQty = (grnQty + srQty + transferInQty + saleMaterialReturnQty) - (saleQty + prQty + dispatchQty + issuanceQty + transferOutQty);
            //  decimal remainingValue = ((grnQty * purchaseRate) + (srQty * srRate) + (transferOutQty * transferOutRate) + (saleMaterialReturnQty * saleMaterialReturnRate)) - ((saleQty * issueRate) + (issuanceQty * issuanceRate) + (prQty * prRate) + (dispatchQty * dispatchRate) + (transferInQty * transferInRate));
            decimal incomingValue = (grnQty * purchaseRate) + (srQty * srRate) + (transferInQty * transferInRate) + (saleMaterialReturnQty * saleMaterialReturnRate);
            decimal outgoingValue = (saleQty * issueRate) + (issuanceQty * issuanceRate) + (transferOutQty * transferOutRate) + (prQty * prRate) + (dispatchQty * dispatchRate);

            // Calculate remaining value
            decimal remainingValue = incomingValue - outgoingValue;

            // Ensure remainingValue is non-negative
            remainingValue = remainingValue < 0 ? 0 : remainingValue;
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
