using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Payroll.SalaryHead.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.SalaryHead.Handler
{
    public class DeleteSalaryHeadHandler : IRequestHandler<DeleteSalaryHeadCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public DeleteSalaryHeadHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteSalaryHeadCommand request, CancellationToken cancellationToken)
        {
            var salaryHead = await unitOfWork.Repository<Entities.Models.SalaryHead>().GetFirstAsync(x => x.Id == request.Id);
            if (salaryHead == null)
            {
                return false;
            }

            salaryHead.IsDelete = true;
            salaryHead.IsActive = false;
            salaryHead.ModifiedById = this.sessionProvider.Session.LoggedInUserId;
            salaryHead.DeleteDate = DateTime.Now;

            unitOfWork.Repository<Entities.Models.SalaryHead>().Update(salaryHead);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
