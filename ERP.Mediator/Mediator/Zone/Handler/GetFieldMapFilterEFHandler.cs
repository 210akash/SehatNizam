using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Zone.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Zone.Handler
{
    public class GetFieldMapFilterEFHandler : IRequestHandler<GetFieldMapFilterEFQuery, GetFieldMapFilterEF>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetFieldMapFilterEFHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetFieldMapFilterEF> Handle(GetFieldMapFilterEFQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Region, bool>> predicateRegion = x =>
            x.IsActive == true
            && x.IsDelete == false
            && (request.RegionId != null ? (request.RegionId == 0 || x.Id == request.RegionId) : false)
            ;
            Expression<Func<Entities.Models.Region, object>>[] includesRegion = { };
            var entityRegion = unitOfWork.Repository<Entities.Models.Region>().GetPagingWhereAsNoTrackingAsync(predicateRegion, null, null, null, null, includesRegion);
            var mapperRegion = mapper.Map<List<GetRegion>>(entityRegion.Item1);


            Expression<Func<Entities.Models.Zone, bool>> predicateZone = x =>
            x.IsActive == true
            && x.IsDelete == false
            && (request.RegionId != null ? (request.RegionId == 0 || x.RegionId == request.RegionId) : false)
            && (request.ZoneId != null ? (request.ZoneId == 0 || x.Id == request.ZoneId) : false)
            ;
            Expression<Func<Entities.Models.Zone, object>>[] includesZone = { };
            var entityZone = unitOfWork.Repository<Entities.Models.Zone>().GetPagingWhereAsNoTrackingAsync(predicateZone, null, null, null, null, includesZone);
            var mapperZone = mapper.Map<List<GetZone>>(entityZone.Item1);


            Expression<Func<Entities.Models.Area, bool>> predicateArea = x =>
            x.IsActive == true
            && x.IsDelete == false
            && (request.RegionId != null ? (request.RegionId == 0 || x.Zone.RegionId == request.RegionId) : false)
            && (request.ZoneId != null ? (request.ZoneId == 0 || x.ZoneId == request.ZoneId) : false)
            && (request.AreaId != null ? (request.AreaId == 0 || x.Id == request.AreaId) : false)
            ;
            Expression<Func<Entities.Models.Area, object>>[] includesArea = { };
            var entityArea = unitOfWork.Repository<Entities.Models.Area>().GetPagingWhereAsNoTrackingAsync(predicateArea, null, null, null, null, includesArea);
            var mapperArea = mapper.Map<List<GetArea>>(entityArea.Item1);


            Expression<Func<Entities.Models.Territory, bool>> predicateTerritory = x =>
            x.IsActive == true
            && x.IsDelete == false
            && (request.RegionId != null ? (request.RegionId == 0 || x.Area.Zone.RegionId == request.RegionId) : false)
            && (request.ZoneId != null ? (request.ZoneId == 0 || x.Area.ZoneId == request.ZoneId) : false)
            && (request.AreaId != null ? (request.AreaId == 0 || x.AreaId == request.AreaId) : false)
            && (request.TerritoryId != null ? (request.TerritoryId == 0 || x.Id == request.TerritoryId) : false)
            ;
            Expression<Func<Entities.Models.Territory, object>>[] includesTerritory = { };
            var entityTerritory = unitOfWork.Repository<Entities.Models.Territory>().GetPagingWhereAsNoTrackingAsync(predicateTerritory, null, null, null, null, includesTerritory);
            var mapperTerritory = mapper.Map<List<GetTerritory>>(entityTerritory.Item1);



            Expression<Func<Entities.Models.Dealership, bool>> predicateDealership = x =>
            x.IsActive == true
            && x.IsDelete == false
            && (request.RegionId != null ? (request.RegionId == 0 || x.Territory.Area.Zone.RegionId == request.RegionId) : false)
            && (request.ZoneId != null ? (request.ZoneId == 0 || x.Territory.Area.ZoneId == request.ZoneId) : false)
            && (request.AreaId != null ? (request.AreaId == 0 || x.Territory.AreaId == request.AreaId) : false)
            && (request.TerritoryId != null ? (request.TerritoryId == 0 || x.TerritoryId == request.TerritoryId) : false)
            && (request.DealershipId != null ? (request.DealershipId == 0 || x.Id == request.DealershipId) : false)
            && (request.TerritoryId != null ? (request.TerritoryId == 0 || x.TerritoryId == request.TerritoryId) : false)
            ;
            Expression<Func<Entities.Models.Dealership, object>>[] includesDealership = { };
            var entityDealership = unitOfWork.Repository<Entities.Models.Dealership>().GetPagingWhereAsNoTrackingAsync(predicateDealership, null, null, null, null, includesDealership);
            var mapperDealership = mapper.Map<List<GetDealership>>(entityDealership.Item1);



            Expression<Func<Entities.Models.Shop, bool>> predicateShop = x =>
            x.IsActive == true
            && x.IsDelete == false
            && (request.RegionId != null ? (request.RegionId == 0 || x.Territory.Area.Zone.RegionId == request.RegionId) : false)
            && (request.ZoneId != null ? (request.ZoneId == 0 || x.Territory.Area.ZoneId == request.ZoneId) : false)
            && (request.AreaId != null ? (request.AreaId == 0 || x.Territory.AreaId == request.AreaId) : false)
            && (request.TerritoryId != null ? (request.TerritoryId == 0 || x.TerritoryId == request.TerritoryId) : false)
            && (request.ShopId != null ? (request.ShopId == 0 || x.Id == request.ShopId) : false)
            && (request.TerritoryId != null ? (request.TerritoryId == 0 || x.TerritoryId == request.TerritoryId) : false)
            //&& (request.ZoneId != null ? (request.ZoneId == 0 || x.Territory.ZoneId == request.ZoneId) : false)
            ;
            Expression<Func<Entities.Models.Shop, object>>[] includesShop = { };
            var entityShop = unitOfWork.Repository<Entities.Models.Shop>().GetPagingWhereAsNoTrackingAsync(predicateShop, null, null, null, null, includesShop);
            var mapperShop = mapper.Map<List<GetShop>>(entityShop.Item1);



            GetFieldMapFilterEF lObj = new GetFieldMapFilterEF();
            lObj.RegionList = mapperRegion;
            lObj.ZoneList = mapperZone;
            lObj.AreaList = mapperArea;
            lObj.TerritoryList = mapperTerritory;
            lObj.DealershipList = mapperDealership;
            lObj.ShopList = mapperShop;
            return lObj;
        }


    }
}
