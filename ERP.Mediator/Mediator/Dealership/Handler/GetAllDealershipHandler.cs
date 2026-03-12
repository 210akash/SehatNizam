using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Dealership.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Dealership.Handler
{
    public class GetAllDealershipHandler : IRequestHandler<GetAllDealershipQuery, Tuple<IEnumerable<GetDealership>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllDealershipHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetDealership>, long>> Handle(GetAllDealershipQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Dealership, bool>> predicate = x =>(request.RegionId == 0 || x.Territory.Area.Zone.RegionId == request.RegionId)
            && (request.ZoneId == 0 || x.Territory.Area.ZoneId == request.ZoneId)
            && (request.AreaId == 0 || x.Territory.AreaId == request.AreaId)
            && (request.TerritoryId == 0 || x.TerritoryId == request.TerritoryId)
            && (request.DealershipTypeId == 0 || x.DealershipTypeId == request.DealershipTypeId)
            && x.Id != 7
            ;

            Expression<Func<Entities.Models.Dealership, object>>[] includes = {
                x => x.Attachments,
                //x => x.Territory,
                //x => x.Territory.Area,
                //x => x.Territory.Area.Zone,
                //x => x.Territory.Area.Zone.Region
            };

            Expression<Func<Entities.Models.Dealership, object>> OrderBy = null;
            Expression<Func<Entities.Models.Dealership, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.Dealership>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var dealership = mapper.Map<IEnumerable<GetDealership>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetDealership>, long>(dealership, entity.Item2);
        }
    }
}
