using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Payroll.EmployeeSalary.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.EmployeeSalary.Handler
{
    public class SaveEmployeeSalaryHandler : IRequestHandler<SaveEmployeeSalaryCommand, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public SaveEmployeeSalaryHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<int> Handle(SaveEmployeeSalaryCommand request, CancellationToken cancellationToken)
        {
            // Validation
            if (request.EmployeeId == 0 || request.SalaryHeadId == 0 || request.Amount < 0)
            {
                return 400; // Bad Request
            }

            Entities.Models.EmployeeSalary employeeSalary;

            if (request.Id > 0)
            {
                // Update existing - but we prefer creating new record with new EffectiveFrom
                employeeSalary = await unitOfWork.Repository<Entities.Models.EmployeeSalary>().GetByIdAsync(request.Id);
                if (employeeSalary == null)
                {
                    return 404; // Not Found
                }

                mapper.Map(request, employeeSalary);
                employeeSalary.UpdatedById = this.sessionProvider.Session.LoggedInUserId;
                employeeSalary.UpdatedDate = DateTime.Now;

                unitOfWork.Repository<Entities.Models.EmployeeSalary>().Update(employeeSalary);
            }
            else
            {
                // Check for duplicate active record
                var exists = await unitOfWork.Repository<Entities.Models.EmployeeSalary>()
                    .AnyAsync(x => x.EmployeeId == request.EmployeeId 
                        && x.SalaryHeadId == request.SalaryHeadId 
                        && x.IsActive 
                        && !x.IsDelete);

                // If exists, mark old as inactive and create new
                if (exists)
                {
                    var oldRecords = await unitOfWork.Repository<Entities.Models.EmployeeSalary>()
                        .GetWhereAsync(x => x.EmployeeId == request.EmployeeId 
                            && x.SalaryHeadId == request.SalaryHeadId 
                            && x.IsActive 
                            && !x.IsDelete);

                    foreach (var old in oldRecords)
                    {
                        old.IsActive = false;
                        old.UpdatedById = this.sessionProvider.Session.LoggedInUserId;
                        old.UpdatedDate = DateTime.Now;
                        unitOfWork.Repository<Entities.Models.EmployeeSalary>().Update(old);
                    }
                }

                // Create new
                employeeSalary = mapper.Map<Entities.Models.EmployeeSalary>(request);
                employeeSalary.CreatedById = this.sessionProvider.Session.LoggedInUserId;
                employeeSalary.CompanyId = this.sessionProvider.Session.CompanyId;

                await unitOfWork.Repository<Entities.Models.EmployeeSalary>().AddAsync(employeeSalary);
            }

            await unitOfWork.CompleteAsync();
            return 200; // Success
        }
    }
}
