using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeBank.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeBank.Handler
{
    public class GetAllEmployeeBankHandler : IRequestHandler<GetAllEmployeeBankQuery, Tuple<IEnumerable<GetEmployeeBank>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllEmployeeBankHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetEmployeeBank>, long>> Handle(GetAllEmployeeBankQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.EmployeeBank, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.EmployeeBank, object>>[] includes = {
                x => x.CreatedBy,
            };

            Expression<Func<Entities.Models.EmployeeBank, object>> OrderBy = null;
            Expression<Func<Entities.Models.EmployeeBank, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.EmployeeBank>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var employeeBank = mapper.Map<IEnumerable<GetEmployeeBank>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetEmployeeBank>, long>(employeeBank, entity.Item2);
        }
    }
}
