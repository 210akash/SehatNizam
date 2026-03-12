using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Area.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Area.Handler
{
    public class GetAllAreaHandler : IRequestHandler<GetAllAreaQuery, Tuple<IEnumerable<GetArea>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllAreaHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetArea>, long>> Handle(GetAllAreaQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Area, bool>> predicate = x => x.IsActive == true
            && (request.RegionId == 0 || x.Zone.RegionId == request.RegionId)
            && (request.ZoneId == 0 || x.ZoneId == request.ZoneId)
            ;

            Expression<Func<Entities.Models.Area, object>>[] includes = {
                x => x.Zone,
                x => x.Zone.Region,
            };

            Expression<Func<Entities.Models.Area, object>> OrderBy = null;
            Expression<Func<Entities.Models.Area, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.Area>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var area = mapper.Map<IEnumerable<GetArea>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetArea>, long>(area, entity.Item2);
        }
    }
}
