using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Region.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Region.Handler
{
    public class GetAllRegionHandler : IRequestHandler<GetAllRegionQuery, Tuple<IEnumerable<GetRegion>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllRegionHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetRegion>, long>> Handle(GetAllRegionQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Region, bool>> predicate = x => x.IsActive == true
            && (string.IsNullOrEmpty(request.Name) || x.Name.ToLower().Contains(request.Name.ToLower()))
            ;

            Expression<Func<Entities.Models.Region, object>>[] includes = {x => x.Zone};

            Expression<Func<Entities.Models.Region, object>> OrderBy = null;
            Expression<Func<Entities.Models.Region, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.Region>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var region = mapper.Map<IEnumerable<GetRegion>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetRegion>, long>(region, entity.Item2);
        }
    }
}
