using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.SalesTarget.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SalesTarget.Handler
{
    public class GetTargetsByTerritoryIdHandler : IRequestHandler<GetTargetsByTerritoryIdQuery, GetTerritoryTarget>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetTargetsByTerritoryIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetTerritoryTarget> Handle(GetTargetsByTerritoryIdQuery request, CancellationToken cancellationToken)
        {
            var RSM = await unitOfWork.Repository<AspNetRoles>().GetFirstAsNoTrackingAsync(x => x.Name == "ZSM");
            var SalesSupervisor = await unitOfWork.Repository<AspNetRoles>().GetFirstAsNoTrackingAsync(x => x.Name == "ASE");
            var Admin = await unitOfWork.Repository<AspNetRoles>().GetFirstAsNoTrackingAsync(x => x.Name == "Admin");

            //var territoryDsfList = await unitOfWork.Repository<Entities.Models.UserTerritory>()
            //    .GetAsync(x => x.TerritoryId == request.TerritoryId && x.IsActive == true && x.User.AspNetUserRoles.FirstOrDefault().RoleId != RSM.Id && x.User.AspNetUserRoles.FirstOrDefault().RoleId != SalesSupervisor.Id && x.User.AspNetUserRoles.FirstOrDefault().RoleId != Admin.Id, null, null, "User.AspNetUserRoles");

            //var territoryTargetsByZoneId = await unitOfWork.Repository<Entities.Models.SalesTarget>()
            //    .GetAsync(x => x.IsActive == true && x.DSFId != null && x.TerritoryId == request.TerritoryId && x.ZoneId != null && x.TargetMonth.Month == request.TargetMonth.Month, null, null, "Territory");

            #region territoryTargets

            //var territoryTargetsofThisMonth = await unitOfWork.Repository<Entities.Models.SalesTarget>()
            //    .GetAsync(x => x.IsActive == true && x.IsDelete == false && x.DSFId == null && x.TerritoryId == request.TerritoryId && x.ZoneId != null && x.TargetMonth.Month == request.TargetMonth.Month, null, null, "Territory");

            var achievedTargetsofThisMonth = await unitOfWork.Repository<Entities.Models.Order>()
               .GetAsync(
               x => x.IsActive == true &&
               x.IsDelete == false &&
               x.DSFId == null &&
               x.Dealership.TerritoryId == request.TerritoryId &&
               x.CreatedDate.Value.Month == request.TargetMonth.Month &&
               x.OrderStatusId == (long)OrderStatusEnum.OrderReceived,
               null,
               null,
               "Dealership,OrderItems"); // Make sure to include "OrderItems" in the include if it's not already

            // Sum the ShippedQuantity from all OrderItems of the achievedTargetsofThisMonth
            int shippedQuantity = achievedTargetsofThisMonth
                .SelectMany(x => x.OrderItems)   // Flatten the OrderItems collection across all orders
                .Sum(y => y.ShippedQuantity).Value;    // Sum the ShippedQuantity from each OrderItem


            //var territoryTargets = mapper.Map<GetSalesTarget>(territoryTargetsofThisMonth.FirstOrDefault());
            //territoryTargets.AchievedTarget = shippedQuantity;
            #endregion

            #region DFS targets

            //var currentTargetByUserId = territoryTargetsByZoneId
            //    .GroupBy(x => x.DSFId)
            //    .Select(g => g.FirstOrDefault())
            //    .ToDictionary(x => x.DSFId, x => x);

            //List<GetSalesTarget> DSFTargetList = new();
            //foreach (var item in territoryDsfList)
            //{
            //    GetSalesTarget DSFTarget = new();
            //    DSFTarget = mapper.Map<GetSalesTarget>(currentTargetByUserId.TryGetValue(item.UserId, out var target) ? target : null);


            //    var achievedTargetsofThisMonth1 = await unitOfWork.Repository<Entities.Models.Order>()
            //     .GetAsync(
            //     x => x.IsActive == true &&
            //        x.IsDelete == false &&
            //        x.DSFId == item.UserId &&
            //        x.Shop.TerritoryId == request.TerritoryId &&
            //        x.CreatedDate.Value.Month == request.TargetMonth.Month &&
            //        x.OrderStatusId == (long)OrderStatusEnum.Received,
            //     null,
            //     null,
            //     "Shop,OrderItems"); // Make sure to include "OrderItems" in the include if it's not already

            //    // Sum the ShippedQuantity from all OrderItems of the achievedTargetsofThisMonth
            //    int shippedQuantityDfs = achievedTargetsofThisMonth1
            //        .SelectMany(x => x.OrderItems)   // Flatten the OrderItems collection across all orders
            //        .Sum(y => y.ShippedQuantity).Value;    // Sum the ShippedQuantity from each OrderItem
            //    if (DSFTarget != null)
            //    {
            //        DSFTarget.AchievedTarget = shippedQuantityDfs;
            //        DSFTargetList.Add(DSFTarget);
            //    }
            //}

            #endregion

            GetTerritoryTarget getTerritoryTarget = new();
            //getTerritoryTarget.TerritoryTarget = territoryTargets;
            //getTerritoryTarget.Target = DSFTargetList;

            return getTerritoryTarget;
        }
    }
}
