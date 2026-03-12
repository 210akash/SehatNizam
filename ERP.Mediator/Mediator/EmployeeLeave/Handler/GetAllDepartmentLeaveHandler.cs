using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.EmployeeLeave.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeave.Handler
{
    public class GetAllDepartmentLeaveHandler : IRequestHandler<GetAllDepartmentLeaveQuery, Tuple<IEnumerable<GetEmployeeLeave>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllDepartmentLeaveHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetEmployeeLeave>, long>> Handle(GetAllDepartmentLeaveQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.EmployeeLeave, bool>> predicate;
            Expression<Func<Entities.Models.EmployeeLeave, object>>[] includes = {
                x => x.CreatedBy,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.ModifiedBy,
                x => x.EmployeeGroupLeaveTypeDetail,
                x => x.EmployeeGroupLeaveTypeDetail.EmployeeLeaveType,
                x => x.Employee,
                x => x.Employee.EmployeeDesignation,
                x => x.Employee.Department,
                x => x.Status,
            };

            predicate = x => x.IsActive == true
                && (request.StatusId == 0 || x.StatusId == request.StatusId)
                && x.CreatedDate >= request.FDate.Value
                && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                && (request.EmployeeId == null || x.EmployeeId == request.EmployeeId)
                && (x.Employee.DepartmentId == sessionProvider.Session.DepartmentId);

            Expression<Func<Entities.Models.EmployeeLeave, object>> OrderBy = null;
            Expression<Func<Entities.Models.EmployeeLeave, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.EmployeeLeave>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var employeeLeave = mapper.Map<IEnumerable<GetEmployeeLeave>>(entity.Item1);
            return new Tuple<IEnumerable<GetEmployeeLeave>, long>(employeeLeave, entity.Item2);
        }
    }
}
