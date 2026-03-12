using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PriceGroup.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.PriceGroup.Handler
{
    public class GetAllDistributorByGroupIdHandler : IRequestHandler<GetAllDistributorByGroupIdQuery, List<GetAllDistributorByGroupId>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllDistributorByGroupIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<List<GetAllDistributorByGroupId>> Handle(GetAllDistributorByGroupIdQuery request, CancellationToken cancellationToken)
        {
            GetAllDistributorByGroupId lstObjGetAllDistributorByGroupId = new GetAllDistributorByGroupId();
            Expression<Func<Entities.Models.Dealership, bool>> predicate = x => x.IsActive == true && x.IsDelete == false && x.Id != 7 && x.DealershipTypeId != 2;
            Expression<Func<Entities.Models.Dealership, object>>[] includes = {

                x => x.Territory,
                x => x.DealershipType,
                x => x.Territory.Area,
                x => x.Territory.Area.Zone,
                x => x.Territory.Area.Zone.Region,
                x=>x.DistributorPriceGroup.Where(y=> y.IsActive == true)
            };

            Expression<Func<Entities.Models.Dealership, object>> OrderBy = null;
            Expression<Func<Entities.Models.Dealership, object>> OrderByDesc = x => x.Id;

            List<string> thenInclude = new List<string>();
            thenInclude.Add("DistributorPriceGroup.PriceGroup");

            var entity = unitOfWork.Repository<Entities.Models.Dealership>().GetPagingWhereAsNoTrackingAsync(predicate, null, OrderBy, OrderByDesc, thenInclude, includes);

            var allDistributors = entity.Item1.Select(dealership => new GetAllDistributorByGroupId
            {
                DealershipId = dealership.Id,
                DealershipName = dealership.Name,
                TerritoryName = dealership.Territory.Name,
                AreaName = dealership.Territory.Area.Name,
                ZoneName = dealership.Territory.Area.Zone.Name,
                RegionName = dealership.Territory.Area.Zone.Region.Name,

                // Fix: Ensures it only returns true if there exists an active record with Id != request.Id
                IsOccupiedInOtherGroup = dealership.DistributorPriceGroup
                                        .Any(dpg => dpg.IsActive == true && dpg.PriceGroupId != request.Id),

                // Correct condition for IsSelected
                IsSelected = dealership.DistributorPriceGroup
                                        .Any(dpg => dpg.IsActive == true && dpg.PriceGroupId == request.Id),

                // Fetch Group Name safely
                GroupName = dealership.DistributorPriceGroup
                                        .Where(y => y.IsActive == true)
                                        .Select(y => y.PriceGroup.Title)
                                        .FirstOrDefault()
            }).ToList();


            return allDistributors;
        }
    }
}
