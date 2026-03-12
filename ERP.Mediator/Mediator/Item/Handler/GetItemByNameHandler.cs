using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Item.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Item.Handler
{
    public class GetItemByNameHandler : IRequestHandler<GetItemByNameQuery, List<GetItem>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetItemByNameHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetItem>> Handleold(GetItemByNameQuery request, CancellationToken cancellationToken)
        {
            // Fetch items
            var items = await unitOfWork.Repository<Entities.Models.Item>().GetAsync(
                y => y.CompanyId == sessionProvider.Session.CompanyId &&
                     (request.StoreId == 0 || y.ItemType.SubCategory.Category.CategoryStores.Any(s => s.StoreId == request.StoreId)) &&
                     (request.name == null || y.Name.ToLower().Contains(request.name.ToLower()) || y.Code.ToLower().Contains(request.name.ToLower())),
                null, null, "UOM", 0, 10);

            var itemIds = items.Select(i => i.Id).ToList();

               var weightedAvgRates = (
                   from poDetail in unitOfWork.Repository<PurchaseOrderDetail>().GetAll().AsQueryable()
                   join po in unitOfWork.Repository<Entities.Models.PurchaseOrder>().GetAll().AsQueryable()
                       on poDetail.PurchaseOrderId equals po.Id
                   join demandDetail in unitOfWork.Repository<PurchaseDemandDetail>().GetAll().AsQueryable()
                       on poDetail.PurchaseDemandDetailId equals demandDetail.Id
                   where po.CompanyId == sessionProvider.Session.CompanyId
                         && itemIds.Contains(demandDetail.ItemId)
                         && po.IsActive
                         && po.StatusId == 3
                   select new
                   {
                       ItemId = demandDetail.ItemId,
                       UnitRate = poDetail.UnitRate,
                       Quantity = poDetail.Quantity
                   }
               )
               .AsEnumerable()
               .GroupBy(x => x.ItemId)
               .Select(g => new
               {
                   ItemId = g.Key,
                   WeightedAvgRate = Math.Round(g.Sum(x => x.UnitRate * x.Quantity) / g.Sum(x => x.Quantity), 2)
               })
               .ToDictionary(x => x.ItemId, x => x.WeightedAvgRate);

            // Map items
            var _Item = mapper.Map<List<GetItem>>(items);

            // Assign LastPurchaseRate
            foreach (var item in _Item)
            {
                if (weightedAvgRates.TryGetValue(item.Id, out var rate)) // Use TryGetValue for safe access
                {
                    item.Rate = rate;
                }

                //Random rnd = new Random();
                //decimal month = rnd.Next(50, 500);  // creates a number between 1 and 12
                //item.Rate = month;
            }

            return _Item;
        }

        public async Task<List<GetItem>> Handle(GetItemByNameQuery request, CancellationToken cancellationToken)
        {
            // Fetch items
            var items = await unitOfWork.Repository<Entities.Models.Item>().GetAsync(
                y => y.CompanyId == sessionProvider.Session.CompanyId &&
                     (request.StoreId == 0 || y.ItemType.SubCategory.Category.CategoryStores.Any(s => s.StoreId == request.StoreId)) &&
                     (request.name == null || y.Name.ToLower().Contains(request.name.ToLower()) || y.Code.ToLower().Contains(request.name.ToLower())),
                null, null, "UOM,ItemType,ItemType.SubCategory,ItemType.SubCategory.Category,ItemType.SubCategory.Category.CategoryStores", 0, 20);

            var itemIds = items.Select(i => i.Id).ToList();

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
                      && itemIds.Contains(item.Id)
                select new
                {
                    ItemId = item.Id,
                    Rate = pod.UnitRate,
                   // Quantity = grnd.Received + item.OpeningQty
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
                        && itemIds.Contains(smDetail.ItemId)
                        && sm.IsActive
                        && smDetail.IsActive
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
                        && smDetail.IsActive
                        && smDetail.Quantity > 0
                        && itemIds.Contains(smDetail.IndentRequestDetail.ItemId)
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
                from prd  in unitOfWork.Repository<PurchaseReturnDetail>().GetAll().AsQueryable()
                join pr in unitOfWork.Repository<Entities.Models.PurchaseReturn>().GetAll().AsQueryable()
                    on  prd.PurchaseReturnId equals pr.Id
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
                      && pr.StatusId == 3
                      && itemIds.Contains(item.Id)
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
                join Ddo in unitOfWork.Repository<Entities.Models.DispatchOrder>().GetAll().AsQueryable()
                    on dd.DispatchOrderId equals Ddo.Id
                join dispatch in unitOfWork.Repository<ERP.Entities.Models.Dispatch>().GetAll().AsQueryable()
                 on Ddo.DispatchId equals dispatch.Id
                join oi in unitOfWork.Repository<OrderItems>().GetAll().AsQueryable()
                    on dd.OrderItemId equals oi.Id
                where dispatch.IsActive
                      && Ddo.IsActive
                      && dd.Quantity > 0
                      && itemIds.Contains(oi.ItemId)
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
                      && itemIds.Contains(oi.ItemId)
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
                        && itemIds.Contains(smrDetail.SaleMaterialDetail.ItemId)
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

            // Map items
            var _Item = mapper.Map<List<GetItem>>(items);

            // Assign LastPurchaseRate
            foreach (var item in _Item)
            {
                // Get GRN values
                //(decimal purchaseRate, decimal grnQty) = weightedAvgRatesGrn.TryGetValue(item.Id, out var grnData) ? grnData : (0m, 0m);

                //// Get Sale values
                // (decimal issueRate, decimal saleQty) = weightedAvgRatesSaleMaterial.TryGetValue(item.Id, out var saleData) ? saleData : (0m, 0m);

                //// Get Sale values
                //(decimal prRate, decimal prQty) = weightedAvgRatesPR.TryGetValue(item.Id, out var prData) ? prData : (0m, 0m);

                // Get rates and quantities
                (decimal purchaseRate, decimal grnQty) = weightedAvgRatesGrn.TryGetValue(item.Id, out var grnData) ? grnData : (0m, 0m);
                (decimal issuanceRate, decimal issuanceQty) = weightedAvgRatesissuance.TryGetValue(item.Id, out var issuanceData) ? issuanceData : (0m, 0m);
                (decimal issueRate, decimal saleQty) = weightedAvgRatesSaleMaterial.TryGetValue(item.Id, out var saleData) ? saleData : (0m, 0m);
                (decimal prRate, decimal prQty) = weightedAvgRatesPR.TryGetValue(item.Id, out var prData) ? prData : (0m, 0m);
                (decimal srRate, decimal srQty) = weightedAvgRatesSR.TryGetValue(item.Id, out var srData) ? srData : (0m, 0m);
                (decimal dispatchRate, decimal dispatchQty) = weightedAvgRatesDispatch.TryGetValue(item.Id, out var dispatchData) ? dispatchData : (0m, 0m);
                (decimal saleMaterialReturnRate, decimal saleMaterialReturnQty) = weightedAvgRatesSaleMaterialReturn.TryGetValue(item.Id, out var SaleMaterialReturnData) ? SaleMaterialReturnData : (0m, 0m);

                // Compute remaining stock and value
                decimal remainingQty = (grnQty + srQty +  saleMaterialReturnQty) - (saleQty + prQty + dispatchQty + issuanceQty);
                decimal incomingValue = (grnQty * purchaseRate) + (srQty * srRate) + (saleMaterialReturnQty * saleMaterialReturnRate);
                decimal outgoingValue = (saleQty * issueRate) + (issuanceQty * issuanceRate) + (prQty * prRate) + (dispatchQty * dispatchRate);

                // Calculate remaining value
                decimal remainingValue = incomingValue - outgoingValue;

                // Ensure remainingValue is non-negative
                remainingValue = remainingValue < 0 ? 0 : remainingValue;
                decimal weightedAvgRate = remainingQty > 0 ? Math.Round(remainingValue / remainingQty, 2) : 0;

                // Compute remaining stock and value
                //decimal remainingQty = grnQty - (saleQty + prQty);
                //decimal remainingValue = (grnQty * purchaseRate) - ((saleQty * issueRate) + (prQty * prRate));
                //decimal weightedAvgRate = remainingQty > 0 ? Math.Round(remainingValue / remainingQty, 2) : 0;
                item.StockQty = remainingQty;
                item.Rate = weightedAvgRate;
            }
            return _Item;
        }
    }
}
