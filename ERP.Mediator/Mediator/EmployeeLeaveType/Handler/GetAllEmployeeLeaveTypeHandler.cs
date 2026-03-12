using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeLeaveType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveType.Handler
{
    public class GetAllEmployeeLeaveTypeHandler : IRequestHandler<GetAllEmployeeLeaveTypeQuery, Tuple<IEnumerable<GetEmployeeLeaveType>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllEmployeeLeaveTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetEmployeeLeaveType>, long>> Handle(GetAllEmployeeLeaveTypeQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.EmployeeLeaveType, bool>> predicate = x => x.IsActive == true;
            Expression<Func<Entities.Models.EmployeeLeaveType, object>>[] includes = {
                x => x.CreatedBy,
            };

            Expression<Func<Entities.Models.EmployeeLeaveType, object>> OrderBy = null;
            Expression<Func<Entities.Models.EmployeeLeaveType, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.EmployeeLeaveType>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var employeeLeaveType = mapper.Map<IEnumerable<GetEmployeeLeaveType>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetEmployeeLeaveType>, long>(employeeLeaveType, entity.Item2);
        }
    }
}
