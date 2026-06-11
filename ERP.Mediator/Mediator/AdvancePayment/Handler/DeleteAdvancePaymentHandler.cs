using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AdvancePayments.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AdvancePayments.Handler
{
    public class DeleteAdvancePaymentsHandler : IRequestHandler<DeleteAdvancePaymentsQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public DeleteAdvancePaymentsHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteAdvancePaymentsQuery request, CancellationToken cancellationToken)
        {
            var AdvancePayments = await unitOfWork.Repository<Entities.Models.AppointmentPayment>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            AdvancePayments.IsDelete = true;
            AdvancePayments.IsActive = false;
            AdvancePayments.ModifiedDate = DateTime.Now;
            AdvancePayments.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.AppointmentPayment>().Update(AdvancePayments);
            return true;
        }
    }
}
