using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Vehicle.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Vehicle.Handler
{
    public class GetAllVehicleHandler : IRequestHandler<GetAllVehicleQuery, Tuple<IEnumerable<GetVehicle>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllVehicleHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetVehicle>, long>> Handle(GetAllVehicleQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Vehicle, bool>> predicate = x => x.IsActive == true
            && (request.DealershipId == 0 || request.DealershipId == null || x.DealershipId == request.DealershipId)
            && (request.RegionId == 0 || request.RegionId == null || x.Dealership.Territory.Area.Zone.RegionId == request.RegionId)
            && (request.ZoneId == 0 || request.ZoneId == null || x.Dealership.Territory.Area.ZoneId == request.ZoneId)
            && (request.AreaId == 0 || request.AreaId == null || x.Dealership.Territory.AreaId == request.AreaId)
            && (request.TerritoryId == 0 || request.TerritoryId == null || x.Dealership.TerritoryId == request.TerritoryId)
            ;

            Expression<Func<Entities.Models.Vehicle, object>>[] includes = {
                x => x.Dealership,
            };

            List<string> thenInclude = new List<string>();

            Expression<Func<Entities.Models.Vehicle, object>> OrderBy = null;
            Expression<Func<Entities.Models.Vehicle, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.Vehicle>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenInclude, includes);
            var vehicle = mapper.Map<IEnumerable<GetVehicle>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetVehicle>, long>(vehicle, entity.Item2);
        }
    }
}
