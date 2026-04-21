using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Payroll.Payroll.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Payroll.Payroll.Handler
{
    public class DeletePayrollHandler : IRequestHandler<DeletePayrollCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public DeletePayrollHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeletePayrollCommand request, CancellationToken cancellationToken)
        {
            var payroll = await unitOfWork.Repository<Entities.Models.Payroll>().GetByIdAsync(request.Id);
            if (payroll == null)
            {
                return false;
            }

            // Cannot delete approved or paid payrolls
            if (payroll.Status == PayrollStatus.Approved || payroll.Status == PayrollStatus.Paid)
            {
                return false;
            }

            // Mark payroll as deleted
            payroll.IsDelete = true;
            payroll.IsActive = false;
            payroll.DeletedById = this.sessionProvider.Session.LoggedInUserId;
            payroll.DeletedDate = DateTime.Now;

            unitOfWork.Repository<Entities.Models.Payroll>().Update(payroll);

            // Also mark all details as deleted
            var details = await unitOfWork.Repository<PayrollDetail>()
                .GetWhereAsync(x => x.PayrollId == request.Id && !x.IsDelete);

            foreach (var detail in details)
            {
                detail.IsDelete = true;
                detail.IsActive = false;
                detail.DeletedById = this.sessionProvider.Session.LoggedInUserId;
                detail.DeletedDate = DateTime.Now;
                unitOfWork.Repository<PayrollDetail>().Update(detail);
            }

            await unitOfWork.CompleteAsync();

            return true;
        }
    }
}
