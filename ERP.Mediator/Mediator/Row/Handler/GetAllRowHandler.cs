using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Row.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Row.Handler
{
    public class GetAllRowHandler : IRequestHandler<GetAllRowQuery, Tuple<IEnumerable<GetRow>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllRowHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetRow>, long>> Handle(GetAllRowQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Row, bool>> predicate = x => x.IsActive == true
            && (string.IsNullOrEmpty(request.Name) || x.Name.ToLower().Contains(request.Name.ToLower()))
            ;

            Expression<Func<Entities.Models.Row, object>>[] includes = {x => x.Rack};

            Expression<Func<Entities.Models.Row, object>> OrderBy = null;
            Expression<Func<Entities.Models.Row, object>> OrderByDescending = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.Row>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDescending, null, includes);
            var Row = mapper.Map<IEnumerable<GetRow>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetRow>, long>(Row, entity.Item2);
        }
    }
}
