using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Payroll.EmployeeSalary.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.EmployeeSalary.Handler
{
    public class GetAllEmployeeSalariesHandler : IRequestHandler<GetAllEmployeeSalariesQuery, IEnumerable<GetEmployeeSalary>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllEmployeeSalariesHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<IEnumerable<GetEmployeeSalary>> Handle(GetAllEmployeeSalariesQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.EmployeeSalary, bool>> predicate = x =>
                x.IsActive == true &&
                x.IsDelete == false ;

            //if (request.EmployeeId.HasValue && request.EmployeeId.Value > 0)
            //{
            //    predicate = x => x.IsActive == true &&
            //        x.IsDelete == false &&
            //        x.EmployeeId == request.EmployeeId.Value;
            //}

            Expression<Func<Entities.Models.EmployeeSalary, object>>[] includes = {
                x => x.SalaryHead
            };

            var employeeSalaries = await unitOfWork.Repository<Entities.Models.EmployeeSalary>()
                .GetAllAsync();

            var result = mapper.Map<IEnumerable<GetEmployeeSalary>>(employeeSalaries.ToList());

            // Fill in names from related entities
            foreach (var item in result)
            {
                var entity = employeeSalaries.FirstOrDefault(x => x.Id == item.Id);
                if (entity != null)
                {
                    item.EmployeeName = entity.Employee?.FirstName ?? "";
                    item.SalaryHeadName = entity.SalaryHead?.Name ?? "";
                    item.SalaryHeadType = entity.SalaryHead?.Type ?? Entities.Models.SalaryHeadType.Earning;
                }
            }

            return result;
        }
    }
}
