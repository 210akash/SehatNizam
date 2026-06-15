using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.AdvancePayments.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Appointment.Handler
{
    public class ConfirmAdvancePaymentHandler : IRequestHandler<ConfirmAdvancePaymentCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public ConfirmAdvancePaymentHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ConfirmAdvancePaymentCommand request, CancellationToken cancellationToken)
        {
            int check = 0;
            // Update related AppointmentPayments
            var payment = unitOfWork.Repository<AdvancePayment>().Find(x => x.Id == request.Id);
            if (payment != null)
            {
                payment.PaymentStatusId = 3; // Set status to 3
                payment.ModifiedById = sessionProvider.Session.LoggedInUserId;
                payment.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<AdvancePayment>().Update(payment);
            }

            check = await unitOfWork.SaveChangesAsync();

            if (check > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
