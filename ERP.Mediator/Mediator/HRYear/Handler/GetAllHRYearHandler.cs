using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.HRYear.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.HRYear.Handler
{
    public class GetAllHRYearHandler : IRequestHandler<GetAllHRYearQuery, Tuple<IEnumerable<GetHRYear>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllHRYearHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetHRYear>, long>> Handle(GetAllHRYearQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.HRYear, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.HRYear, object>>[] includes = {
                x => x.CreatedBy,
            };

            Expression<Func<Entities.Models.HRYear, object>> OrderBy = null;
            Expression<Func<Entities.Models.HRYear, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.HRYear>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var HRYear = mapper.Map<IEnumerable<GetHRYear>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetHRYear>, long>(HRYear, entity.Item2);
        }
    }
}
