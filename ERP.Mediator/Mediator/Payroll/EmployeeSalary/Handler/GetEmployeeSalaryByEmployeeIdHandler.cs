using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Payroll.EmployeeSalary.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.EmployeeSalary.Handler
{
    /// <summary>
    /// Gets the latest active salary records for an employee as of a specific date.
    /// For each salary head, returns the most recent record where EffectiveFrom <= AsOfDate
    /// </summary>
    public class GetEmployeeSalaryByEmployeeIdHandler : IRequestHandler<GetEmployeeSalaryByEmployeeIdQuery, IEnumerable<GetEmployeeSalary>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeSalaryByEmployeeIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<GetEmployeeSalary>> Handle(GetEmployeeSalaryByEmployeeIdQuery request, CancellationToken cancellationToken)
        {
            // Get all non-deleted salary records for this employee that were effective as of the date
            Expression<Func<Entities.Models.EmployeeSalary, bool>> predicate = x =>
                x.EmployeeId == new Guid(request.EmployeeId) &&  
                x.IsActive &&
                !x.IsDelete;

            Expression<Func<Entities.Models.EmployeeSalary, object>>[] includes = {
                x => x.SalaryHead
            };

            var allSalaries = await unitOfWork.Repository<Entities.Models.EmployeeSalary>()
                .GetAllAsync();

            // Group by SalaryHeadId and take the most recent (by EffectiveFrom) for each group
            var latestSalaries = allSalaries
                .GroupBy(x => x.SalaryHeadId)
                .Select(g => g.OrderByDescending(x => x.EffectiveFrom).ThenByDescending(x => x.Id).First())
                .Where(x => x.IsActive) // Only return if the latest is still active
                .ToList();

            var result = mapper.Map<IEnumerable<GetEmployeeSalary>>(latestSalaries);
            return result;
        }
    }
}
