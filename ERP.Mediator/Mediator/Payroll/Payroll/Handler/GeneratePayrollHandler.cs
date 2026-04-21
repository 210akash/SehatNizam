using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Payroll.EmployeeSalary.Query;
using ERP.Mediator.Mediator.Payroll.Payroll.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.Payroll.Handler
{
    /// <summary>
    /// Handler to generate payroll for a specific month and year.
    /// Fetches latest employee salaries and copies them to PayrollDetail as snapshot.
    /// </summary>
    public class GeneratePayrollHandler : IRequestHandler<GeneratePayrollCommand, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        private readonly IMediator mediator;

        public GeneratePayrollHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider, IMediator mediator)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
            this.mediator = mediator;
        }

        public async Task<int> Handle(GeneratePayrollCommand request, CancellationToken cancellationToken)
        {
            // Validation
            if (request.Month < 1 || request.Month > 12 || request.Year < 2000)
            {
                return 400; // Bad Request
            }

            // Check if payroll already exists for this month/year
            var existingPayroll = await unitOfWork.Repository<Entities.Models.Payroll>()
                .FirstOrDefaultAsync(x => x.Month == request.Month 
                    && x.Year == request.Year 
                    && x.CompanyId == this.sessionProvider.Session.CompanyId
                    && !x.IsDelete);

            if (existingPayroll != null)
            {
                if (existingPayroll.Status == PayrollStatus.Approved || existingPayroll.Status == PayrollStatus.Paid)
                {
                    return 409; // Conflict - Cannot regenerate approved/paid payroll
                }
                // If draft, delete old details and regenerate
                var oldDetails = await unitOfWork.Repository<PayrollDetail>()
                    .GetWhereAsync(x => x.PayrollId == existingPayroll.Id);
                
                foreach (var detail in oldDetails)
                {
                    detail.IsDelete = true;
                    unitOfWork.Repository<PayrollDetail>().Update(detail);
                }
            }
            else
            {
                // Create new payroll
                existingPayroll = new Entities.Models.Payroll
                {
                    Month = request.Month,
                    Year = request.Year,
                    Status = PayrollStatus.Draft,
                    CompanyId = this.sessionProvider.Session.CompanyId,
                    CreatedById = this.sessionProvider.Session.LoggedInUserId,
                    IsActive = true
                };
                await unitOfWork.Repository<Entities.Models.Payroll>().AddAsync(existingPayroll);
                await unitOfWork.CompleteAsync(); // Save to get the ID
            }

            // Get all active employees
            var employees = await unitOfWork.Repository<Employee>()
                .GetWhereAsync(x => x.IsActive && !x.IsDelete && x.CompanyId == this.sessionProvider.Session.CompanyId);

            // Last day of the month for salary calculation
            var payrollDate = new DateTime(request.Year, request.Month, 1).AddMonths(1).AddDays(-1);

            int recordsCreated = 0;

            // For each employee, fetch their latest salary and create payroll details
            foreach (var employee in employees)
            {
                var latestSalaries = await mediator.Send(new GetLatestEmployeeSalariesQuery
                {
                    EmployeeId = employee.Id,
                    AsOfDate = payrollDate
                }, cancellationToken);

                foreach (var salary in latestSalaries)
                {
                    var payrollDetail = new PayrollDetail
                    {
                        PayrollId = existingPayroll.Id,
                        EmployeeId = employee.Id,
                        SalaryHeadId = salary.SalaryHeadId,
                        Amount = salary.Amount,
                        SalaryHeadName = salary.SalaryHeadName,
                        SalaryHeadType = salary.SalaryHeadType,
                        CompanyId = this.sessionProvider.Session.CompanyId,
                        CreatedById = this.sessionProvider.Session.LoggedInUserId,
                        IsActive = true
                    };

                    await unitOfWork.Repository<PayrollDetail>().AddAsync(payrollDetail);
                    recordsCreated++;
                }
            }

            await unitOfWork.CompleteAsync();
            return 200; // Success
        }
    }
}
