using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.BusinessModels.Enums;
using ERP.API.Extensions;
using System.Collections.Generic;
using ERP.Entities.Models;
using ERP.Repositories.UnitOfWork;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using ERP.Services.Interfaces;
using ERP.Mediator.Mediator.Shop.Query;
using ERP.Mediator.Mediator.Region.Query;
using ERP.Mediator.Mediator.Dealership.Query;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ERP.Mediator.Mediator.Zone.Query;
using ERP.Mediator.Mediator.Territory.Query;
using ERP.Mediator.Mediator.Area.Query;
using System.Linq.Expressions;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using static ERP.Mediator.Mediator.Ledger.Handler.GetCustomerBalanceHandler;
using Microsoft.SqlServer.Management.XEvent;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeoMapController : ControllerBase
    {
        private readonly IMediator mediator;
        private string SecurityToken = "6XesrAM2Nu";
        private readonly IUnitOfWork unitOfWork;
        private readonly IUnitOfWorkDapper unitOfWorkDapper;
        private readonly IBlobService blobService;
        private readonly IMapper mapper;
        private readonly IConfiguration _configuration;
        private readonly string Localcontainer;

        public GeoMapController(IUnitOfWork unitOfWork, IMediator mediator, IBlobService blobService, IMapper mapper, IConfiguration configuration, IUnitOfWorkDapper unitOfWorkDapper)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.blobService = blobService;
            this.mapper = mapper;
            _configuration = configuration;
            Localcontainer = _configuration["LocalBlob:BlobContainerName"];
            this.unitOfWorkDapper = unitOfWorkDapper;
        }

        #region Map API

        [HttpGet]
        [Route("IsValidToken")]
        private bool IsValidToken(string requestToken)
        {
            if (string.IsNullOrWhiteSpace(requestToken) || requestToken != SecurityToken)
            {
                return false;
            }
            else if (requestToken == SecurityToken)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        [Route("GetFieldMapFilter")]
        public async Task<ActionResult<GetFieldMapFilterSP>> GetFieldMapFilter(GetFieldMapFilterSPQuery getFieldMapFilterQuery)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                return await this.mediator.Send(getFieldMapFilterQuery);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAllRegion")]
        public async Task<ActionResult<IEnumerable<GetRegionLite>>> GetAll(GetAllRegionQuery getAllRegionQuery)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                Expression<Func<Region, bool>> predicate = x => x.IsActive == true;
                var entity = unitOfWork.Repository<Region>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, null, null);
                var region = mapper.Map<IEnumerable<GetRegionLite>>(entity.Item1.ToList()).ToList();
                return region;
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetZoneByRegionId")]
        public async Task<ActionResult<List<GetZoneLite>>> GetZoneByRegionId(long regionId)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                Expression<Func<Zone, bool>> predicate = y => y.RegionId == regionId && y.IsActive == true;
                var entity = unitOfWork.Repository<Zone>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, null, null);
                var _zone = mapper.Map<List<GetZoneLite>>(entity.Item1.ToList());
                return _zone;
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAreaByZoneId")]
        public async Task<ActionResult<List<GetAreaLite>>> GetAreaByZoneId(long zoneId)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                Expression<Func<Area, bool>> predicate = y => y.ZoneId == zoneId && y.IsActive == true;
                var entity = unitOfWork.Repository<Area>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, null, null);
                var _area = mapper.Map<List<GetAreaLite>>(entity.Item1.ToList());
                return _area;
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetTerritoryByAreaId")]
        public async Task<ActionResult<List<GetTerritoryLite>>> GetTerritoryByAreaId(long AreaId)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var territory = await unitOfWork.Repository<Territory>().GetAsync(y => y.AreaId == AreaId && y.IsActive == true);
                var _territory = mapper.Map<List<GetTerritoryLite>>(territory);
                return _territory;
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDealershipByTerritoryId")]
        public async Task<ActionResult<List<GetDealershipLite>>> GetDealershipByTerritoryId(long territoryId)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var dealership = await unitOfWork.Repository<Dealership>().GetAsync(y => y.DealershipTypeId == 1 && y.TerritoryId == territoryId && y.IsActive == true, null, null,
                    "Territory,Territory.Area,Territory.Area.Zone,Territory.Area.Zone.Region");

                var _dealership = mapper.Map<List<GetDealershipLite>>(dealership);
                return _dealership;
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetShopsByTerritoryId")]
        public async Task<ActionResult<List<GetShopLite>>> GetShopsByTerritoryId(long territoryId)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var shops = await unitOfWork.Repository<Shop>().GetAsync(y => y.TerritoryId == territoryId, null, null,
                    "Territory,Territory.Area,Territory.Area.Zone,Territory.Area.Zone.Region,Territory.Dealership");

                var _shops = mapper.Map<List<GetShopLite>>(shops);
                return _shops;
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllDealerships")]
        public async Task<ActionResult<IEnumerable<GetDealershipLite>>> GetAllDealerships()
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var result = await unitOfWork.Repository<Dealership>().GetAsync(x => x.IsActive == true && x.IsDelete == false, null, null,
                    "Territory,Territory.Area,Territory.Area.Zone,Territory.Area.Zone.Region");

                var map = mapper.Map<List<GetDealershipLite>>(result);
                return map;
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllShops")]
        public async Task<ActionResult<IEnumerable<GetShopLite>>> GetAllShops()
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var reportQuery = $"GetAllShops";

                var result = await unitOfWorkDapper.Repository<GetShopLite>()
                    .QueryAsync<GetShopLite>(reportQuery);

                return result.ToList();
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetKhilafatReportYearWise")]
        public async Task<ActionResult<List<GetDistributorsSaleReport>>> GetKhilafatReportYearWise(long year)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                //var reportQuery = $"GetDistributorsSaleReport";
                var reportQuery = $"GetDistributorsSaleReport @Year = '{year}'";

                var result = await unitOfWorkDapper.Repository<GetDistributorsSaleReport>()
                    .QueryAsync<GetDistributorsSaleReport>(reportQuery);

                return result.ToList();
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetTop50DistributorsSale")]
        public async Task<ActionResult<List<GetTop50DistributorsSale>>> GetTop50DistributorsSale(long year, long month)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var reportQuery = $"GetTop50DistributorsSale @Year = '{year}'," +
                              $"@Month = '{month}'";

                var result = await unitOfWorkDapper.Repository<GetTop50DistributorsSale>()
                    .QueryAsync<GetTop50DistributorsSale>(reportQuery);

                return result.ToList();
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetTopSellingSKUs")]
        public async Task<ActionResult<List<GetTopSellingSKUs>>> GetTopSellingSKUs(DateTime? FDate, DateTime? TDate)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var reportQuery = $"GetTopSellingSKUs @FDate = '{FDate}'," +
                              $"@TDate = '{TDate}'";

                var result = await unitOfWorkDapper.Repository<GetTopSellingSKUs>()
                    .QueryAsync<GetTopSellingSKUs>(reportQuery);

                return result.ToList();
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetShopsByIdLite")]
        public async Task<ActionResult<GetShopLite>> GetShopsByIdLite(long shopId)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                Expression<Func<Entities.Models.Shop, bool>> predicateShop = x =>
                    x.IsActive == true
                    && x.IsDelete == false
                    && x.Id == shopId;

                Expression<Func<Entities.Models.Shop, object>>[] includesShop = {
                x => x.Attachments,
                x => x.Territory,
                x => x.Territory.Dealership,
                x => x.Territory.Area,
                x => x.Territory.Area.Zone,
                x => x.Territory.Area.Zone.Region
            };

                var entityShop = unitOfWork.Repository<Entities.Models.Shop>().GetPagingWhereAsNoTrackingAsync(predicateShop, null, null, null, null, includesShop);
                var mapperShop = mapper.Map<List<GetShopLite>>(entityShop.Item1);
                return mapperShop.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDealershipByIdLite")]
        public async Task<ActionResult<GetDealershipLite>> GetDealershipByIdLite(long dealershipId)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                Expression<Func<Entities.Models.Dealership, bool>> predicateDealership = x =>
                x.IsActive == true
                && x.IsDelete == false
                && x.Id == dealershipId;
                ;

                Expression<Func<Entities.Models.Dealership, object>>[] includesDealership = {
                    x => x.Territory,
                    x => x.Territory.Area,
                    x => x.Territory.Area.Zone,
                    x => x.Territory.Area.Zone.Region
                };

                var entityDealership = unitOfWork.Repository<Entities.Models.Dealership>().GetPagingWhereAsNoTrackingAsync(predicateDealership, null, null, null, null, includesDealership);
                var mapperDealership = mapper.Map<List<GetDealershipLite>>(entityDealership.Item1);
                return mapperDealership.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetTop50DistributorsSaleSecond")]
        public async Task<ActionResult<List<GetTop50DistributorsSale>>> GetTop50DistributorsSaleSecond(long year, long month)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var reportQuery = $"GetTop50DistributorsSaleSecond @Year = '{year}'," +
                              $"@Month = '{month}'";

                var result = await unitOfWorkDapper.Repository<GetTop50DistributorsSale>()
                    .QueryAsync<GetTop50DistributorsSale>(reportQuery);

                return result.ToList();
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetKhilafatReportYearWiseSecond")]
        public async Task<ActionResult<List<GetDistributorsSaleReport>>> GetKhilafatReportYearWiseSecond(long year)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                //var reportQuery = $"GetDistributorsSaleReport";
                var reportQuery = $"GetDistributorsSaleReportSecond @Year = '{year}'";

                var result = await unitOfWorkDapper.Repository<GetDistributorsSaleReport>()
                    .QueryAsync<GetDistributorsSaleReport>(reportQuery);

                return result.ToList();
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetTopSellingSKUsSecond")]
        public async Task<ActionResult<List<GetTopSellingSKUs>>> GetTopSellingSKUsSecond(DateTime? FDate, DateTime? TDate)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var reportQuery = $"GetTopSellingSKUsSecond @FDate = '{FDate}'," +
                              $"@TDate = '{TDate}'";

                var result = await unitOfWorkDapper.Repository<GetTopSellingSKUs>()
                    .QueryAsync<GetTopSellingSKUs>(reportQuery);

                return result.ToList();
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetOrdersByDistributorByMonth")]
        public async Task<ActionResult<List<GetOrdersByDistributorByMonth>>> GetOrdersByDistributorByMonth(long year, long month, long dealershipId)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var reportQuery = $"GetOrdersByDistributorByMonth @Year = '{year}'," +
                              $"@Month = '{month}', @DealershipId = '{dealershipId}'";

                var result = await unitOfWorkDapper.Repository<GetOrdersByDistributorByMonth>()
                    .QueryAsync<GetOrdersByDistributorByMonth>(reportQuery);

                return result.ToList();
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetSecondOrdersByDistributorByMonth")]
        public async Task<ActionResult<List<GetOrdersByDistributorByMonth>>> GetSecondOrdersByDistributorByMonth(long year, long month, long dealershipId)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var reportQuery = $"GetSecondOrdersByDistributorByMonth @Year = '{year}'," +
                              $"@Month = '{month}', @DealershipId = '{dealershipId}'";

                var result = await unitOfWorkDapper.Repository<GetOrdersByDistributorByMonth>()
                    .QueryAsync<GetOrdersByDistributorByMonth>(reportQuery);

                return result.ToList();
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDistributorByName")]
        public async Task<ActionResult<List<GetDealershipLite>>> GetDistributorByName(string Name)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var dealership = unitOfWork.Repository<Dealership>().GetAsync(y => y.IsActive == true && y.Name.ToLower().Contains(Name.ToLower())).Result.Take(10);

                var _dealership = mapper.Map<List<GetDealershipLite>>(dealership);
                return _dealership;
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetShopByName")]
        public async Task<ActionResult<List<GetShopLite>>> GetShopByName(string Name)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var shop = unitOfWork.Repository<Shop>().GetAsync(y => y.IsActive == true && y.Name.ToLower().Contains(Name.ToLower())).Result.Take(10);

                var _shop = mapper.Map<List<GetShopLite>>(shop);
                return _shop;
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetRegionWiseSale")]
        public async Task<ActionResult<List<GetRegionWiseSale>>> GetRegionWiseSale(DateTime? FDate, DateTime? TDate)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var reportQuery = $"GetRegionWiseSale @FDate = '{FDate}'," +
                              $"@TDate = '{TDate}'";

                var result = await unitOfWorkDapper.Repository<GetRegionWiseSale>()
                    .QueryAsync<GetRegionWiseSale>(reportQuery);

                return result.ToList();
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetZoneWiseSale")]
        public async Task<ActionResult<List<GetZoneWiseSale>>> GetZoneWiseSale(DateTime? FDate, DateTime? TDate, long RegionId)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var reportQuery = $"GetZoneWiseSale @FDate = '{FDate}', " +
                  $"@TDate = '{TDate}', " +
                  $"@RegionId = '{RegionId}'";

                var result = await unitOfWorkDapper.Repository<GetZoneWiseSale>()
                    .QueryAsync<GetZoneWiseSale>(reportQuery);

                return result.ToList();
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetSaleReportSKUWise")]
        public async Task<ActionResult<List<ItemMonthlySalesVM>>> GetSaleReportSKUWise(int Year)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var reportQuery = $"GetSaleReportSKUWise @Year = '{Year}'";

                var result = await unitOfWorkDapper.Repository<ItemMonthlySalesVM>()
                    .QueryAsync<ItemMonthlySalesVM>(reportQuery);

                return result.ToList();
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        #endregion
    }
}