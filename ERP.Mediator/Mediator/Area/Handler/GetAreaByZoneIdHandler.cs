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
    public class GetAreaByZoneIdHandler : IRequestHandler<GetAreaByZoneIdQuery, List<GetArea>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAreaByZoneIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetArea>> Handle(GetAreaByZoneIdQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Area, bool>> predicate = y => y.ZoneId == request.ZoneId && y.IsActive == true;

            Expression<Func<Entities.Models.Area, object>>[] includes = { x => x.Territory };

            var entity = unitOfWork.Repository<Entities.Models.Area>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, null, includes);

            var _area = mapper.Map<List<GetArea>>(entity.Item1.ToList());
            return _area;
        }
    }
}
