using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.LabOrderType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.LabOrderType.Handler
{
    public class GetAllLabOrderTypeHandler : IRequestHandler<GetAllLabOrderTypeQuery, Tuple<IEnumerable<GetLabOrderType>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllLabOrderTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetLabOrderType>, long>> Handle(GetAllLabOrderTypeQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.LabOrderType, bool>> predicate =
                x => x.IsActive == true;

            Expression<Func<Entities.Models.LabOrderType, object>>[] includes = { x => x.Service };
            Expression<Func<Entities.Models.LabOrderType, object>> orderBy = null;
            Expression<Func<Entities.Models.LabOrderType, object>> orderByDesc = x => x.Id;

            var result = unitOfWork.Repository<Entities.Models.LabOrderType>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, orderBy, orderByDesc, null, includes);

            var mapped = mapper.Map<IEnumerable<GetLabOrderType>>(result.Item1);
            return new Tuple<IEnumerable<GetLabOrderType>, long>(mapped, result.Item2);
        }
    }
}
