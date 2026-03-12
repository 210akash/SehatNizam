using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeType.Handler
{
    public class GetAllEmployeeTypeHandler : IRequestHandler<GetAllEmployeeTypeQuery, Tuple<IEnumerable<GetEmployeeType>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllEmployeeTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetEmployeeType>, long>> Handle(GetAllEmployeeTypeQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.EmployeeType, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.EmployeeType, object>>[] includes = {
                x => x.CreatedBy,
            };

            Expression<Func<Entities.Models.EmployeeType, object>> OrderBy = null;
            Expression<Func<Entities.Models.EmployeeType, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.EmployeeType>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var employeeType = mapper.Map<IEnumerable<GetEmployeeType>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetEmployeeType>, long>(employeeType, entity.Item2);
        }
    }
}
