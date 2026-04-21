using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Payroll.Payroll.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.Payroll.Handler
{
    public class SavePayrollHandler : IRequestHandler<SavePayrollCommand, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public SavePayrollHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<int> Handle(SavePayrollCommand request, CancellationToken cancellationToken)
        {
            // Validation
            if (request.Month < 1 || request.Month > 12 || request.Year < 2000)
            {
                return 400; // Bad Request
            }

            Entities.Models.Payroll payroll;

            if (request.Id > 0)
            {
                // Update existing
                payroll = await unitOfWork.Repository<Entities.Models.Payroll>().GetByIdAsync(request.Id);
                if (payroll == null || payroll.IsDelete)
                {
                    return 404; // Not Found
                }

                // Prevent editing if already paid
                if (payroll.Status == PayrollStatus.Paid && request.Status != PayrollStatus.Paid)
                {
                    return 403; // Forbidden - Cannot modify paid payroll
                }

                mapper.Map(request, payroll);
                payroll.UpdatedById = this.sessionProvider.Session.LoggedInUserId;
                payroll.UpdatedDate = DateTime.Now;

                unitOfWork.Repository<Entities.Models.Payroll>().Update(payroll);
            }
            else
            {
                // Check for duplicate
                var exists = await unitOfWork.Repository<Entities.Models.Payroll>()
                    .AnyAsync(x => x.Month == request.Month 
                        && x.Year == request.Year 
                        && x.CompanyId == this.sessionProvider.Session.CompanyId
                        && !x.IsDelete);

                if (exists)
                {
                    return 409; // Conflict - Payroll already exists for this month/year
                }

                // Create new
                payroll = mapper.Map<Entities.Models.Payroll>(request);
                payroll.CreatedById = this.sessionProvider.Session.LoggedInUserId;
                payroll.CompanyId = this.sessionProvider.Session.CompanyId;
                payroll.IsActive = true;

                await unitOfWork.Repository<Entities.Models.Payroll>().AddAsync(payroll);
            }

            await unitOfWork.CompleteAsync();
            return 200; // Success
        }
    }
}
