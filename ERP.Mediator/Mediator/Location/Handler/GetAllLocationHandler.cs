using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Location.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Location.Handler
{
    public class GetAllLocationHandler : IRequestHandler<GetAllLocationQuery, Tuple<IEnumerable<GetLocation>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllLocationHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetLocation>, long>> Handle(GetAllLocationQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Location, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.Location, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company
            };

            Expression<Func<Entities.Models.Location, object>> OrderBy = null;
            Expression<Func<Entities.Models.Location, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Location>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var Location = mapper.Map<IEnumerable<GetLocation>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetLocation>, long>(Location, entity.Item2);
        }
    }
}
