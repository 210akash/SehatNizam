using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Store.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Store.Handler
{
    public class GetAllStoreHandler : IRequestHandler<GetAllStoreQuery, Tuple<IEnumerable<GetStore>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllStoreHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetStore>, long>> Handle(GetAllStoreQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Store, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.Store, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Location,
                x => x.Location.Company
            };

            Expression<Func<Entities.Models.Store, object>> OrderBy = null;
            Expression<Func<Entities.Models.Store, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Store>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var Store = mapper.Map<IEnumerable<GetStore>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetStore>, long>(Store, entity.Item2);
        }
    }
}
