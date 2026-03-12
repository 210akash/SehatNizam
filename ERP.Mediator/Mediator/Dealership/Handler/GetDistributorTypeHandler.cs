using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountCategory.Query;
using ERP.Mediator.Mediator.Dealership.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Dealership.Handler
{
    public class GetDistributorTypeHandler : IRequestHandler<GetDistributorTypeQuery, List<GetDistributorType>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetDistributorTypeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mapper = mapper;
        }
        public async Task<List<GetDistributorType>> Handle(GetDistributorTypeQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.DealershipType, bool>> predicate = x => x.IsActive == true && x.Id != 2
             ;

            Expression<Func<Entities.Models.DealershipType, object>>[] includes = {
            };

            Expression<Func<Entities.Models.DealershipType, object>> OrderBy = null;
            Expression<Func<Entities.Models.DealershipType, object>> OrderByDesc = x => x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.DealershipType>().GetPagingWhereAsNoTrackingAsync(predicate, null, OrderBy, OrderByDesc, null, includes);
            var DealershipType = mapper.Map<IEnumerable<GetDistributorType>>(entity.Item1.ToList()).ToList();
            return DealershipType;
        }

    }
}
