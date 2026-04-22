using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Payroll.SalaryHead.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.SalaryHead.Handler
{
    public class SaveSalaryHeadHandler : IRequestHandler<SaveSalaryHeadCommand, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public SaveSalaryHeadHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<int> Handle(SaveSalaryHeadCommand request, CancellationToken cancellationToken)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return 400; // Bad Request
            }

            Entities.Models.SalaryHead salaryHead;

            if (request.Id > 0)
            {
                // Update existing
                salaryHead = await unitOfWork.Repository<Entities.Models.SalaryHead>().GetFirstAsync(x => x.Id == request.Id);
                if (salaryHead == null)
                {
                    return 404; // Not Found
                }

                mapper.Map(request, salaryHead);
                salaryHead.ModifiedById = this.sessionProvider.Session.LoggedInUserId;
                salaryHead.ModifiedDate = DateTime.Now;

                unitOfWork.Repository<Entities.Models.SalaryHead>().Update(salaryHead);
            }
            else
            {
                // Check for duplicate
                var exists = await unitOfWork.Repository<Entities.Models.SalaryHead>()
                    .GetFirstAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive && !x.IsDelete);

                if (exists != null)
                {
                    return 409; // Conflict
                }

                // Create new
                salaryHead = mapper.Map<Entities.Models.SalaryHead>(request);
                salaryHead.CreatedById = this.sessionProvider.Session.LoggedInUserId;
                salaryHead.IsActive = true;
                await unitOfWork.Repository<Entities.Models.SalaryHead>().AddAsync(salaryHead);
            }

            await unitOfWork.SaveChangesAsync();
            return 200; // Success
        }
    }
}
