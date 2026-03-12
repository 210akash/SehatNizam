using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeWorkSiteType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeWorkSiteType.Handler
{
    public class GetAllEmployeeWorkSiteTypeHandler : IRequestHandler<GetAllEmployeeWorkSiteTypeQuery, Tuple<IEnumerable<GetEmployeeWorkSiteType>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllEmployeeWorkSiteTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetEmployeeWorkSiteType>, long>> Handle(GetAllEmployeeWorkSiteTypeQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.EmployeeWorkSiteType, bool>> predicate = x => x.IsActive == true;

            Expression<Func<Entities.Models.EmployeeWorkSiteType, object>>[] includes = {
                x => x.CreatedBy,
            };

            Expression<Func<Entities.Models.EmployeeWorkSiteType, object>> OrderBy = null;
            Expression<Func<Entities.Models.EmployeeWorkSiteType, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.EmployeeWorkSiteType>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var employeeDesignation = mapper.Map<IEnumerable<GetEmployeeWorkSiteType>>(entity.Item1);

            return new Tuple<IEnumerable<GetEmployeeWorkSiteType>, long>(employeeDesignation, entity.Item2);
        }
    }
}
