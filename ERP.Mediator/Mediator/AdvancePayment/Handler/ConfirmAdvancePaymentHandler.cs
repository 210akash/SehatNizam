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
    public class ConfirmAdvancePaymentHandler : IRequestHandler<ConfirmAdvancePaymentCommand, Tuple<long, string>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public ConfirmAdvancePaymentHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<long, string>> Handle(ConfirmAdvancePaymentCommand request, CancellationToken cancellationToken)
        {
            int check = 0;
            // Update related AppointmentPayments
            var payment = unitOfWork.Repository<AppointmentPayment>().Find(x => x.Id == request.Id);
            if (payment != null)
            {
                payment.PaymentStatusId = 3; // Set status to 3
                payment.ModifiedById = sessionProvider.Session.LoggedInUserId;
                payment.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<AppointmentPayment>().Update(payment);
            }

            check = await unitOfWork.SaveChangesAsync();

            if (check > 0)
            {
                return new Tuple<long, string>(200, "Service Confirmed Successfully!");
            }
            else
            {
                return new Tuple<long, string>(500, "Error Confirming, Please contact system admin!");
            }
        }
    }
}
