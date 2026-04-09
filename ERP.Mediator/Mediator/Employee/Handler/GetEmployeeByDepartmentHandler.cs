using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Employee.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.Employee.Handler
{
    public class GetEmployeeByDepartmentHandler : IRequestHandler<GetEmployeeByDepartment, List<GetEmployee>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeByDepartmentHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetEmployee>> Handle(GetEmployeeByDepartment request, CancellationToken cancellationToken)
        {
            var Employee = await unitOfWork.Repository<Entities.Models.AspNetUsers>().GetAsync(y =>
               y.IsActive == true && y.DepartmentId == request.DepartmentId, null, null, "EmployeeDesignation,EmployeeShift");

            var result = await unitOfWork.Repository<AspNetUsers>()
            .GetQueryable()
            .Where(e =>
             e.IsActive &&
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
