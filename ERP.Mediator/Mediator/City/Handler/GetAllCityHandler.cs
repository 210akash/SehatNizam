using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.City.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.City.Handler
{
    public class GetAllCityHandler : IRequestHandler<GetAllCityQuery, Tuple<IEnumerable<GetCity>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllCityHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetCity>, long>> Handle(GetAllCityQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.City, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.City, object>>[] includes = {
                x => x.CreatedBy,
            };

            Expression<Func<Entities.Models.City, object>> OrderBy = null;
            Expression<Func<Entities.Models.City, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.City>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var city = mapper.Map<IEnumerable<GetCity>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetCity>, long>(city, entity.Item2);
        }
    }
}
