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

namespace ERP.Mediator.Mediator.Account.Handler
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
            var Employee = await unitOfWork.Repository<Entities.Models.AspNetUsers>().GetAsync(y =>
               y.IsActive == true && y.DepartmentId == request.DepartmentId && (y.FirstName.ToLower().Contains(request.Name.Trim().ToLower()) || y.LastName.ToLower().Contains(request.Name.Trim().ToLower())), null, null, "EmployeeDesignation");
           // var _Account = mapper.Map<List<GetEmployee>>(Employee);

            var result = await unitOfWork.Repository<AspNetUsers>()
    .GetQueryable()
    .Where(e =>
        e.IsActive &&
        e.DepartmentId == request.DepartmentId &&
        (e.FirstName.Contains(request.Name) || e.LastName.Contains(request.Name)))
    .Select(e => new GetEmployee
    {
        Id = e.Id,
        HrCode = e.HrCode,
        WorkLocation = e.WorkLocation,
        PhoneNumber = e.PhoneNumber,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Designation = e.EmployeeDesignation.Name,
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
