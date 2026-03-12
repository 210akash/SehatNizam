using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Dashboard.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Dashboard.Handler
{
    public class GetHRDashboardHandler : IRequestHandler<GetHRDashboardQuery, GetHRDashboardData>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetHRDashboardHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetHRDashboardData> Handle(GetHRDashboardQuery request, CancellationToken cancellationToken)
        {
            var employees = await unitOfWork.Repository<AspNetUsers>().GetAsync(x => x.IsActive && !x.IsDelete && x.IsEmployee, null, null, "AspNetUserRoles,AspNetUserRoles.Role,Department");

            var employeeByDepartment = employees.Where(x => x.DepartmentId != null).GroupBy(x => x.Department.Name)
                .Select(g => new GetDepartmentWiseCount
                    {
                        Department = g.Key,
                        Count = g.Count()
                    }
                ).ToList();

            GetHRDashboardData getHRDashboardData = new GetHRDashboardData();
            getHRDashboardData.TotalEmployee = employees.Count();
            getHRDashboardData.NewThisMonth = employees.Where(x => x.JoinDate != null && x.JoinDate.Value.Month == DateTime.Now.Month).Count();
            getHRDashboardData.ResignedThisMonth = employees.Where(x => x.ResignDate != null).Count();
            getHRDashboardData.SaleEmployees = employees.Where(x => (x.DepartmentId == 12 || x.DepartmentId == 23) &&  x.EmployeeWorkSiteTypeId == 1 &&  x.IsEmployee).Count();
            getHRDashboardData.SaleFieldWorkers = employees.Where(x => (x.DepartmentId == 12 || x.DepartmentId == 23) && x.EmployeeWorkSiteTypeId == 2 && x.IsEmployee).Count();
            getHRDashboardData.GetDepartmentWiseCount = employeeByDepartment;
            return getHRDashboardData;
        }
    }
}
