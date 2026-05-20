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
    public class GetEmployeeByNameHandler : IRequestHandler<GetEmployeeByNameQuery, List<GetEmployee>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetEmployee>> Handle(GetEmployeeByNameQuery request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Repository<AspNetUsers>()
    .GetQueryable()
    .Where(e =>
        e.IsActive &&
        e.IsEmployee &&
        (request.DepartmentId == null || e.DepartmentId == request.DepartmentId) &&
        (e.FirstName.Trim().ToLower().Contains(request.Name.Trim().ToLower()) || e.LastName.Trim().ToLower().Contains(request.Name.Trim().ToLower())))
    .Select(e => new GetEmployee
    {
        Id = e.Id,
        HrCode = e.HrCode,
        WorkLocation = e.WorkLocation,
        PhoneNumber = e.PhoneNumber,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Designation = e.EmployeeDesignation.Name,
        Department = e.Department.Name,
        Supervisor = string.Join(", ",
            e.Department.Users
                .Where(u =>
                    u.IsEmployee &&
                    u.AspNetUserRoles.Any(r => r.Role.Name == "Manager"))
                .Select(u => u.FirstName + " " + u.LastName))
    }).ToListAsync();
            var _Account = mapper.Map<List<GetEmployee>>(result);
            return _Account;
        }
    }
}
