using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Employee.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Employee.Handler
{
    public class GetEmployeeByDepartmentHandler : IRequestHandler<GetEmployeeByDepartment, List<GetEmployee>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetEmployeeByDepartmentHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetEmployee>> Handle(GetEmployeeByDepartment request, CancellationToken cancellationToken)
        {
            // Check if the current user's Roles array contains the AccountOwnerRole
            string[] roles = this.sessionProvider.Session.Roles;
            // Check if the current user's roles contain "Manager"
            if (roles.Any(r => r == "Manager"))
            {
                request.DepartmentId = this.sessionProvider.Session.DepartmentId;
            }
            var Employee = await unitOfWork.Repository<AspNetUsers>().GetAsync(y =>
            y.IsActive == true && y.DepartmentId == request.DepartmentId, null, null, "EmployeeDesignation,EmployeeShift");

            var result = await unitOfWork.Repository<AspNetUsers>()
            .GetQueryable()
            .Where(e =>
             e.IsActive &&
             e.IsEmployee &&
             e.DepartmentId == request.DepartmentId)
             .Select(e => new GetEmployee
             {
                 Id = e.Id,
                 HrCode = e.HrCode,
                 PhoneNumber = e.PhoneNumber,
                 FirstName = e.FirstName,
                 LastName = e.LastName,
                 Designation = e.EmployeeDesignation.Name,
                 EmployeeShiftId = e.EmployeeShiftId.Value,
             }).ToListAsync();
            var _Account = mapper.Map<List<GetEmployee>>(result);
            return _Account;
        }
    }
}
