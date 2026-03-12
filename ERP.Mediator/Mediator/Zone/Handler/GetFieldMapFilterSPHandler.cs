using AutoMapper;
using Dapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Zone.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.Extensions.ObjectPool;
using PdfSharpCore.Pdf.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Zone.Handler
{
    public class GetFieldMapFilterSPHandler : IRequestHandler<GetFieldMapFilterSPQuery, GetFieldMapFilterSP>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUnitOfWorkDapper unitOfWorkDapper;
        private readonly IMapper mapper;

        public GetFieldMapFilterSPHandler(IUnitOfWork unitOfWork, IMapper mapper, IUnitOfWorkDapper unitOfWorkDapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.unitOfWorkDapper = unitOfWorkDapper;
        }

        public async Task<GetFieldMapFilterSP> Handle(GetFieldMapFilterSPQuery request, CancellationToken cancellationToken)
        {
            //var query = string.Format($"FieldMapGetRegions @RegionId = '{request.RegionId}'");
            //var response = (unitOfWorkDapper.Repository<GetRegionLite>()
            //.QueryAsync<GetRegionLite>(query)).Result.ToList();

            var regionQuery = "FieldMapGetRegions"; // Stored procedure name or query

            var regionParameters = new DynamicParameters();
            regionParameters.Add("@RegionId", request.RegionId, DbType.Int32);

            var regionResponse = (await unitOfWorkDapper.Repository<GetRegionLite>()
                .QueryAsync<GetRegionLite>(regionQuery, regionParameters, CommandType.StoredProcedure)).ToList();



            var zoneQuery = "FieldMapGetZones"; // Stored procedure name or query

            var zoneParameters = new DynamicParameters();
            zoneParameters.Add("@RegionId", request.RegionId, DbType.Int32);
            zoneParameters.Add("@ZoneId", request.ZoneId, DbType.Int32);

            var zoneResponse = (await unitOfWorkDapper.Repository<GetZoneLite>()
            .QueryAsync<GetZoneLite>(zoneQuery, zoneParameters, CommandType.StoredProcedure)).ToList();



            var areaQuery = "FieldMapGetAreas"; // Stored procedure name or query

            var areaParameters = new DynamicParameters();
            areaParameters.Add("@RegionId", request.RegionId, DbType.Int32);
            areaParameters.Add("@ZoneId", request.ZoneId, DbType.Int32);
            areaParameters.Add("@AreaId", request.AreaId, DbType.Int32);

            var areaResponse = (await unitOfWorkDapper.Repository<GetAreaLite>()
            .QueryAsync<GetAreaLite>(areaQuery, areaParameters, CommandType.StoredProcedure)).ToList();



            var territoriesQuery = "FieldMapGetTerritories"; // Stored procedure name or query

            var territoriesParameters = new DynamicParameters();
            territoriesParameters.Add("@RegionId", request.RegionId, DbType.Int32);
            territoriesParameters.Add("@ZoneId", request.ZoneId, DbType.Int32);
            territoriesParameters.Add("@AreaId", request.AreaId, DbType.Int32);
            territoriesParameters.Add("@TerritoryId", request.TerritoryId, DbType.Int32);

            var territoriesResponse = (await unitOfWorkDapper.Repository<GetTerritoryLite>()
            .QueryAsync<GetTerritoryLite>(territoriesQuery, territoriesParameters, CommandType.StoredProcedure)).ToList();

            List<GetDealershipLite> dealershipResponse = new List<GetDealershipLite>();
            if (request.DealershipEnabled == 1)
            {
                var dealershipQuery = "FieldMapGetDealerships"; // Stored procedure name, use raw SQL for query

                var dealershipParameters = new DynamicParameters();
                dealershipParameters.Add("@RegionId", request.RegionId, DbType.Int32);
                dealershipParameters.Add("@ZoneId", request.ZoneId, DbType.Int32);
                dealershipParameters.Add("@AreaId", request.AreaId, DbType.Int32);
                dealershipParameters.Add("@TerritoryId", request.TerritoryId, DbType.Int32);
                //dealershipParameters.Add("@DealershipId", request.DealershipId, DbType.Int32);

                dealershipResponse = (await unitOfWorkDapper.Repository<GetDealershipLite>()
                    .QueryAsync<GetDealershipLite>(dealershipQuery, dealershipParameters, CommandType.StoredProcedure)).ToList();
            }


            List<GetShopLite> shopsResponse = new List<GetShopLite>();
            if (request.ShopEnabled == 1)
            {
                var shopsQuery = "FieldMapGetShops"; // Stored procedure name or query

                var shopsParameters = new DynamicParameters();
                shopsParameters.Add("@RegionId", request.RegionId, DbType.Int32);
                shopsParameters.Add("@ZoneId", request.ZoneId, DbType.Int32);
                shopsParameters.Add("@AreaId", request.AreaId, DbType.Int32);
                shopsParameters.Add("@TerritoryId", request.TerritoryId, DbType.Int32);
                //shopsParameters.Add("@ShopId", request.ShopId, DbType.Int32);

                shopsResponse = (await unitOfWorkDapper.Repository<GetShopLite>()
                    .QueryAsync<GetShopLite>(shopsQuery, shopsParameters, CommandType.StoredProcedure)).ToList();
            }

            GetFieldMapFilterSP lObj = new GetFieldMapFilterSP();
            lObj.RegionList = regionResponse;
            lObj.ZoneList = zoneResponse;
            lObj.AreaList = areaResponse;
            lObj.TerritoryList = territoriesResponse;
            lObj.DealershipList = dealershipResponse;
            lObj.ShopList = shopsResponse;
            return lObj;
        }
    }
}
