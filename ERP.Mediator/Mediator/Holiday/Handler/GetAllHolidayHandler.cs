using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Holiday.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Holiday.Handler
{
    public class GetAllHolidayHandler : IRequestHandler<GetAllHolidayQuery, Tuple<IEnumerable<GetHoliday>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllHolidayHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetHoliday>, long>> Handle(GetAllHolidayQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Holiday, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.Holiday, object>>[] includes = {
                x => x.CreatedBy,
            };

            Expression<Func<Entities.Models.Holiday, object>> OrderBy = null;
            Expression<Func<Entities.Models.Holiday, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Holiday>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var Holiday = mapper.Map<IEnumerable<GetHoliday>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetHoliday>, long>(Holiday, entity.Item2);
        }
    }
}
