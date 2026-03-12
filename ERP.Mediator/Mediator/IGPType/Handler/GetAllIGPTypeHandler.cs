using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IGPType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IGPType.Handler
{
    public class GetAllIGPTypeHandler : IRequestHandler<GetAllIGPTypeQuery, Tuple<IEnumerable<GetIGPType>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllIGPTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetIGPType>, long>> Handle(GetAllIGPTypeQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.IGPType, bool>> predicate = x => x.IsActive == true;
            Expression<Func<Entities.Models.IGPType, object>>[] includes = {
                x => x.CreatedBy,
            };

            Expression<Func<Entities.Models.IGPType, object>> OrderBy = null;
            Expression<Func<Entities.Models.IGPType, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.IGPType>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var IGPType = mapper.Map<IEnumerable<GetIGPType>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetIGPType>, long>(IGPType, entity.Item2);
        }
    }
}
